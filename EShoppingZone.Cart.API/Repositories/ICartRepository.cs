using EShoppingZone.Cart.API.Entities;

namespace EShoppingZone.Cart.API.Repositories
{
    public interface ICartRepository
    {
        Task<CartEntity?> FindByCartId(int cartId);
    }
}