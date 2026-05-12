using EShoppingZone.Order.API.DTOs;
using EShoppingZone.Order.API.Entities;

namespace EShoppingZone.Order.API.Services
{
    public interface IOrderService
    {
        Task<IList<OrderEntity>> GetAllOrders();
        Task<OrderEntity> PlaceOrder(PlaceOrderDto dto);
        Task<bool> ChangeStatus(int orderId, string status);
        Task DeleteOrder(int orderId);
        Task<IList<OrderEntity>> GetOrderByCustomerId(int customerId);
        Task<IList<OrderEntity>> GetOrderByMerchantId(int merchantId);
        Task<OrderEntity?> GetOrderById(int orderId);
        Task<Dictionary<string, string>> InitiateOnlinePayment(decimal amount, string currency);
        Task<OrderEntity> VerifyAndPlaceOrder(VerifyPaymentDto dto, int customerId);
    }
}