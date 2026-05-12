using EShoppingZone.Cart.API.Data;
using EShoppingZone.Cart.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EShoppingZone.Cart.API.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly CartDbContext _context;

        public CartRepository(CartDbContext context)
        {
            _context = context;
        }

        public async Task<CartEntity?> FindByCartId(int cartId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CartId == cartId);
        }
    }
}