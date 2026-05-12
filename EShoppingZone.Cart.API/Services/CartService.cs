using EShoppingZone.Cart.API.Data;
using EShoppingZone.Cart.API.Entities;
using EShoppingZone.Cart.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EShoppingZone.Cart.API.Services
{
    public class CartService : ICartService
    {
        private readonly CartDbContext _context;
        private readonly ICartRepository _repository;

        public CartService(CartDbContext context, ICartRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        public async Task<CartEntity?> GetCartById(int cartId)
        {
            return await _repository.FindByCartId(cartId);
        }

        public async Task<IList<CartEntity>> GetAllCarts()
        {
            return await _context.Carts.Include(c => c.Items).ToListAsync();
        }

        public async Task<CartEntity> AddCart(int cartId)
        {
            var existing = await _context.Carts.FindAsync(cartId);
            if (existing != null) return existing;

            var cart = new CartEntity { CartId = cartId, TotalPrice = 0 };
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<CartEntity> UpdateCart(CartEntity cart)
        {
            cart.TotalPrice = CartTotal(cart);
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public decimal CartTotal(CartEntity cart)
        {
            return cart.Items.Sum(i => i.Price * i.Quantity);
        }
    }
}