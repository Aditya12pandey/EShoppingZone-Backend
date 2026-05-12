using EShoppingZone.Order.API.Entities;

namespace EShoppingZone.Order.API.Repositories
{
    public interface IOrderRepository
    {
        Task<IList<OrderEntity>> FindByCustomerId(int customerId);
        Task<IList<OrderEntity>> FindByMerchantId(int merchantId);
        Task<OrderEntity?> FindById(int orderId);
    }
}