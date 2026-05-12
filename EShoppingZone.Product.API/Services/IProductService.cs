using EShoppingZone.Product.API.Entities;

namespace EShoppingZone.Product.API.Services
{
    public interface IProductService
    {
        Task AddProducts(ProductEntity product);
        Task<IList<ProductEntity>> GetAllProducts();
        Task<ProductEntity?> GetProductById(int id);
        Task<ProductEntity?> GetProductByName(string name);
        Task<ProductEntity> UpdateProducts(ProductEntity product);
        Task DeleteProductById(int id);
        Task<IList<ProductEntity>> GetProductsByCategory(string category);
        Task<IList<ProductEntity>> GetProductsByType(string type);
    }
}