using EShoppingZone.Cart.API.Entities;

namespace EShoppingZone.Cart.API.Services
{
    public interface ICartService
    {
        Task<CartEntity?> GetCartById(int cartId);
        Task<IList<CartEntity>> GetAllCarts();
        Task<CartEntity> AddCart(int cartId);
        Task<CartEntity> UpdateCart(CartEntity cart);
        decimal CartTotal(CartEntity cart);
    }
}