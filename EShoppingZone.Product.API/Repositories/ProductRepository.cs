using EShoppingZone.Product.API.Data;
using EShoppingZone.Product.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EShoppingZone.Product.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _context;

        public ProductRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<IList<ProductEntity>> FindByProductName(string name)
        {
            return await _context.Products
                .Where(p => p.ProductName.ToLower().Contains(name.ToLower()))
                .ToListAsync();
        }

        public async Task<IList<ProductEntity>> FindByCategory(string category)
        {
            return await _context.Products
                .Where(p => p.Category.ToLower() == category.ToLower())
                .ToListAsync();
        }

        public async Task<IList<ProductEntity>> FindByProductType(string type)
        {
            return await _context.Products
                .Where(p => p.ProductType.ToLower() == type.ToLower())
                .ToListAsync();
        }
    }
}