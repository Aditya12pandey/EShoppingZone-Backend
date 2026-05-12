using EShoppingZone.Product.API.DTOs;
using EShoppingZone.Product.API.Entities;
using EShoppingZone.Product.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShoppingZone.Product.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Authorize(Roles = "MERCHANT")]
        public async Task<IActionResult> AddProducts([FromBody] AddProductDto dto)
        {
            var merchantIdClaim = User.FindFirst("userId")?.Value;
            if (merchantIdClaim == null) return Unauthorized();

            var product = new ProductEntity
            {
                ProductType = dto.ProductType,
                ProductName = dto.ProductName,
                Category = dto.Category,
                Image = dto.Image,
                Price = dto.Price,
                Description = dto.Description,
                Specification = dto.Specification,
                MerchantId = int.Parse(merchantIdClaim),
                Rating = new Dictionary<int, double>(),
                Review = new Dictionary<int, string>()
            };

            await _productService.AddProducts(product);
            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProducts();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductById(id);
            if (product == null) return NotFound(new { message = "Product not found." });
            return Ok(product);
        }

        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetProductByType(string type)
        {
            var products = await _productService.GetProductsByType(type);
            return Ok(products);
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetProductByName(string name)
        {
            var product = await _productService.GetProductByName(name);
            if (product == null) return NotFound(new { message = "Product not found." });
            return Ok(product);
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetProductsByCategory(string category)
        {
            var products = await _productService.GetProductsByCategory(category);
            return Ok(products);
        }

        [HttpPut]
        [Authorize(Roles = "MERCHANT")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDto dto)
        {
            var merchantIdClaim = User.FindFirst("userId")?.Value;
            if (merchantIdClaim == null) return Unauthorized();

            var existing = await _productService.GetProductById(dto.ProductId);
            if (existing == null) return NotFound(new { message = "Product not found." });

            if (existing.MerchantId != int.Parse(merchantIdClaim))
                return StatusCode(403, "You are not authorized to update this product.");

            existing.ProductType = dto.ProductType;
            existing.ProductName = dto.ProductName;
            existing.Category = dto.Category;
            existing.Image = dto.Image;
            existing.Price = dto.Price;
            existing.Description = dto.Description;
            existing.Specification = dto.Specification;

            var updated = await _productService.UpdateProducts(existing);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "MERCHANT")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var merchantIdClaim = User.FindFirst("userId")?.Value;
            if (merchantIdClaim == null) return Unauthorized();

            var existing = await _productService.GetProductById(id);
            if (existing == null) return NotFound(new { message = "Product not found." });

            if (existing.MerchantId != int.Parse(merchantIdClaim))
                return StatusCode(403, "You are not authorized to delete this product.");

            await _productService.DeleteProductById(id);
            return Ok(new { message = "Product deleted successfully." });
        }
    }
}