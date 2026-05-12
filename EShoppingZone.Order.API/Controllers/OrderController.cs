using EShoppingZone.Order.API.DTOs;
using EShoppingZone.Order.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShoppingZone.Order.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET /api/orders (Admin)
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            return Ok(orders);
        }

        // POST /api/orders/place (Customer)
        [HttpPost("place")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (userIdClaim == null) return Unauthorized();
            dto.CustomerId = int.Parse(userIdClaim);

            // Extract JWT token and pass it to service for inter-service call
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader.StartsWith("Bearer "))
                dto.BearerToken = authHeader.Substring(7);

            var order = await _orderService.PlaceOrder(dto);
            return Ok(order);
        }

        // GET /api/orders/customer/{customerId}
        [HttpGet("customer/{customerId}")]
        [Authorize(Roles = "CUSTOMER,ADMIN")]
        public async Task<IActionResult> GetOrderByCustomerId(int customerId)
        {
            var orders = await _orderService.GetOrderByCustomerId(customerId);
            return Ok(orders);
        }

        // GET /api/orders/merchant/{merchantId}
        [HttpGet("merchant/{merchantId}")]
        [Authorize(Roles = "MERCHANT,ADMIN")]
        public async Task<IActionResult> GetOrderByMerchantId(int merchantId)
        {
            var orders = await _orderService.GetOrderByMerchantId(merchantId);
            return Ok(orders);
        }

        // GET /api/orders/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null) return NotFound(new { message = "Order not found." });
            return Ok(order);
        }

        // PUT /api/orders/status (Admin/Merchant)
        [HttpPut("status")]
        [Authorize(Roles = "ADMIN,MERCHANT")]
        public async Task<IActionResult> ChangeStatus([FromBody] ChangeStatusDto dto)
        {
            Console.WriteLine($"DEBUG: ChangeStatus requested for Order {dto.OrderId} to {dto.Status}");
            var result = await _orderService.ChangeStatus(dto.OrderId, dto.Status);
            if (!result) 
            {
                Console.WriteLine($"DEBUG: ChangeStatus FAILED - Order {dto.OrderId} not found.");
                return NotFound(new { message = "Order not found." });
            }
            Console.WriteLine($"DEBUG: ChangeStatus SUCCESS - Order {dto.OrderId} updated.");
            return Ok(new { message = "Order status updated." });
        }

        // DELETE /api/orders/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "CUSTOMER,ADMIN")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _orderService.DeleteOrder(id);
            return Ok(new { message = "Order deleted." });
        }

        // POST /api/orders/initiatePayment (Customer)
        [HttpPost("initiatePayment")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentDto dto)
        {
            try
            {
                var result = await _orderService.InitiateOnlinePayment(dto.Amount, dto.Currency);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST /api/orders/verifyAndPlace (Customer)
        [HttpPost("verifyAndPlace")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> VerifyAndPlace([FromBody] VerifyPaymentDto dto)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (userIdClaim == null) return Unauthorized();

            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader.StartsWith("Bearer "))
                dto.Address = dto.Address;

            try
            {
                var order = await _orderService.VerifyAndPlaceOrder(dto, int.Parse(userIdClaim));
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}