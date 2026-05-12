using EShoppingZone.Order.API.Data;
using EShoppingZone.Order.API.DTOs;
using EShoppingZone.Order.API.Entities;
using EShoppingZone.Order.API.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using Razorpay.Api;

namespace EShoppingZone.Order.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderDbContext _context;
        private readonly IOrderRepository _repository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public OrderService(OrderDbContext context, IOrderRepository repository, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _repository = repository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        public async Task<IList<OrderEntity>> GetAllOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<OrderEntity> PlaceOrder(PlaceOrderDto dto)
        {
            bool walletDeducted = false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Step 1 — Deduct wallet if EWALLET payment
                if (dto.ModeOfPayment.ToUpper() == "EWALLET")
                {
                    var client = _httpClientFactory.CreateClient("WalletApi");

                    if (!string.IsNullOrEmpty(dto.BearerToken))
                        client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", dto.BearerToken);

                    var payDto = new
                    {
                        WalletId = dto.CustomerId,
                        Amount = dto.AmountPaid,
                        Remarks = "Payment for order",
                        OrderId = 0
                    };

                    var content = new StringContent(
                        JsonSerializer.Serialize(payDto),
                        Encoding.UTF8,
                        "application/json"
                    );

                    var payResponse = await client.PostAsync("/api/wallet/payMoney", content);

                    if (!payResponse.IsSuccessStatusCode)
                        throw new Exception("Insufficient wallet balance or wallet payment failed.");

                    // Mark wallet as deducted — needed for compensation if order save fails
                    walletDeducted = true;
                }

                // Step 2 — Save the order
                var order = new OrderEntity
                {
                    OrderDate = DateTime.UtcNow,
                    CustomerId = dto.CustomerId,
                    MerchantId = dto.MerchantId,
                    AmountPaid = dto.AmountPaid,
                    ModeOfPayment = dto.ModeOfPayment.ToUpper(),
                    OrderStatus = "Placed",
                    Quantity = dto.Quantity,
                    ProductName = dto.ProductName,
                    ProductId = dto.ProductId,
                    Address = dto.Address
                };

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                // Step 3 — SAGA COMPENSATION: Refund wallet if money was deducted but order failed
                if (walletDeducted)
                {
                    try
                    {
                        var client = _httpClientFactory.CreateClient("WalletApi");

                        if (!string.IsNullOrEmpty(dto.BearerToken))
                            client.DefaultRequestHeaders.Authorization =
                                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", dto.BearerToken);

                        var refundDto = new
                        {
                            WalletId = dto.CustomerId,
                            Amount = dto.AmountPaid,
                            Remarks = "Refund due to order failure"
                        };

                        var refundContent = new StringContent(
                            JsonSerializer.Serialize(refundDto),
                            Encoding.UTF8,
                            "application/json"
                        );

                        await client.PostAsync("/api/wallet/refund", refundContent);
                    }
                    catch
                    {
                        // Log refund failure — in production this would trigger an alert
                        // For now we rethrow the original exception
                    }
                }

                throw new Exception($"Order failed: {ex.Message}");
            }
        }

        public async Task<bool> ChangeStatus(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;
            order.OrderStatus = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IList<OrderEntity>> GetOrderByCustomerId(int customerId)
        {
            return await _repository.FindByCustomerId(customerId);
        }

        public async Task<IList<OrderEntity>> GetOrderByMerchantId(int merchantId)
        {
            return await _repository.FindByMerchantId(merchantId);
        }

        public async Task<OrderEntity?> GetOrderById(int orderId)
        {
            return await _repository.FindById(orderId);
        }

        public async Task<Dictionary<string, string>> InitiateOnlinePayment(decimal amount, string currency)
        {
            var keyId = _configuration["Razorpay:KeyId"]!;
            var keySecret = _configuration["Razorpay:KeySecret"]!;

            var client = new RazorpayClient(keyId, keySecret);

            var options = new Dictionary<string, object>
            {
                { "amount", (int)(amount * 100) }, // Razorpay expects amount in paise
                { "currency", currency },
                { "receipt", $"order_rcpt_{Guid.NewGuid().ToString()[..8]}" },
                { "payment_capture", 1 }
            };

            var order = client.Order.Create(options);

            return new Dictionary<string, string>
            {
                { "razorpayOrderId", order["id"].ToString() },
                { "amount", order["amount"].ToString() },
                { "currency", order["currency"].ToString() },
                { "keyId", keyId }
            };
        }

        public async Task<OrderEntity> VerifyAndPlaceOrder(VerifyPaymentDto dto, int customerId)
        {
            var keyId = _configuration["Razorpay:KeyId"]!;
            var keySecret = _configuration["Razorpay:KeySecret"]!;

            // Initialize client so Utils.verifyPaymentSignature has access to the secret
            var client = new RazorpayClient(keyId, keySecret);

            // Verify payment signature
            var attributes = new Dictionary<string, string>
            {
                { "razorpay_order_id", dto.RazorpayOrderId },
                { "razorpay_payment_id", dto.RazorpayPaymentId },
                { "razorpay_signature", dto.RazorpaySignature }
            };

            try
            {
                Utils.verifyPaymentSignature(attributes);
            }
            catch
            {
                throw new Exception("Payment verification failed. Invalid signature.");
            }

            // Place order after successful verification
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new OrderEntity
                {
                    OrderDate = DateTime.UtcNow,
                    CustomerId = customerId,
                    MerchantId = dto.MerchantId,
                    AmountPaid = dto.AmountPaid,
                    ModeOfPayment = "ONLINE",
                    OrderStatus = "Placed",
                    Quantity = dto.Quantity,
                    ProductName = dto.ProductName,
                    ProductId = dto.ProductId,
                    Address = dto.Address
                };

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new Exception("Order placement failed after payment verification.");
            }
        }
    }
}