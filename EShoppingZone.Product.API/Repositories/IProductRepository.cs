using EShoppingZone.Product.API.Entities;

namespace EShoppingZone.Product.API.Repositories
{
    public interface IProductRepository
    {
        Task<IList<ProductEntity>> FindByProductName(string name);
        Task<IList<ProductEntity>> FindByCategory(string category);
        Task<IList<ProductEntity>> FindByProductType(string type);
    }
}