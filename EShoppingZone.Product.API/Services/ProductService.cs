using EShoppingZone.Product.API.Data;
using EShoppingZone.Product.API.Entities;
using EShoppingZone.Product.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EShoppingZone.Product.API.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductDbContext _context;
        private readonly IProductRepository _repository;

        public ProductService(ProductDbContext context, IProductRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        public async Task AddProducts(ProductEntity product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<ProductEntity>> GetAllProducts()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<ProductEntity?> GetProductById(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<ProductEntity?> GetProductByName(string name)
        {
            var results = await _repository.FindByProductName(name);
            return results.FirstOrDefault();
        }

        public async Task<ProductEntity> UpdateProducts(ProductEntity product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IList<ProductEntity>> GetProductsByCategory(string category)
        {
            return await _repository.FindByCategory(category);
        }

        public async Task<IList<ProductEntity>> GetProductsByType(string type)
        {
            return await _repository.FindByProductType(type);
        }
    }
}