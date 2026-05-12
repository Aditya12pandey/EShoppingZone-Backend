using EShoppingZone.Order.API.Data;
using EShoppingZone.Order.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EShoppingZone.Order.API.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<IList<OrderEntity>> FindByCustomerId(int customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<IList<OrderEntity>> FindByMerchantId(int merchantId)
        {
            return await _context.Orders
                .Where(o => o.MerchantId == merchantId)
                .ToListAsync();
        }

        public async Task<OrderEntity?> FindById(int orderId)
        {
            return await _context.Orders.FindAsync(orderId);
        }
    }
}