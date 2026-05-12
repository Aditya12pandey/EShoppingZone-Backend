using EShoppingZone.Product.API.Data;
using EShoppingZone.Product.API.DTOs;
using EShoppingZone.Product.API.Entities;
using EShoppingZone.Product.API.Repositories;
using EShoppingZone.Product.API.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace EShoppingZone.Product.API.Tests
{
    [TestFixture]
    public class ProductServiceTests
    {
        private Mock<IProductRepository> _mockRepo;
        private ProductDbContext _context;
        private ProductService _productService;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IProductRepository>();

            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ProductDbContext(options);

            _productService = new ProductService(_context, _mockRepo.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        //  AddProducts Tests 

        [Test]
        public async Task AddProducts_ShouldAddProduct_WhenCalled()
        {
            // Arrange
            var product = new ProductEntity
            {
                ProductName = "Wireless Headphones",
                ProductType = "Electronics",
                Category = "Audio",
                Price = 1999.99m,
                Description = "High quality wireless headphones",
                MerchantId = 1,
                Rating = new Dictionary<int, double>(),
                Review = new Dictionary<int, string>(),
                Image = new List<string>(),
                Specification = new Dictionary<string, string>()
            };

            // Act
            await _productService.AddProducts(product);

            // Assert
            var result = await _context.Products.FirstOrDefaultAsync();
            result.Should().NotBeNull();
            result!.ProductName.Should().Be("Wireless Headphones");
        }

        //  GetAllProducts Tests 

        [Test]
        public async Task GetAllProducts_ShouldReturnAllProducts()
        {
            // Arrange
            _context.Products.AddRange(
                new ProductEntity
                {
                    ProductName = "Product 1", ProductType = "Type1", Category = "Cat1",
                    Price = 100, Description = "Desc1", MerchantId = 1,
                    Rating = new(), Review = new(), Image = new List<string>(), Specification = new()
                },
                new ProductEntity
                {
                    ProductName = "Product 2", ProductType = "Type2", Category = "Cat2",
                    Price = 200, Description = "Desc2", MerchantId = 1,
                    Rating = new(), Review = new(), Image = new List<string>(), Specification = new()
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.GetAllProducts();

            // Assert
            result.Should().HaveCount(2);
        }

        //  GetProductById Tests 

        [Test]
        public async Task GetProductById_ShouldReturnProduct_WhenExists()
        {
            // Arrange
            var product = new ProductEntity
            {
                ProductName = "Test Product", ProductType = "Type1", Category = "Cat1",
                Price = 100, Description = "Desc1", MerchantId = 1,
                Rating = new(), Review = new(), Image = new List<string>(), Specification = new()
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.GetProductById(product.ProductId);

            // Assert
            result.Should().NotBeNull();
            result!.ProductName.Should().Be("Test Product");
        }

        [Test]
        public async Task GetProductById_ShouldReturnNull_WhenNotFound()
        {
            // Act
            var result = await _productService.GetProductById(999);

            // Assert
            result.Should().BeNull();
        }

        //  GetProductByName Tests 

        [Test]
        public async Task GetProductByName_ShouldReturnProduct_WhenExists()
        {
            // Arrange
            var products = new List<ProductEntity>
            {
                new ProductEntity
                {
                    ProductName = "Wireless Headphones", ProductType = "Electronics",
                    Category = "Audio", Price = 1999, Description = "Good headphones",
                    MerchantId = 1, Rating = new(), Review = new(),
                    Image = new List<string>(), Specification = new()
                }
            };

            _mockRepo.Setup(r => r.FindByProductName("Wireless Headphones"))
                     .ReturnsAsync(products);

            // Act
            var result = await _productService.GetProductByName("Wireless Headphones");

            // Assert
            result.Should().NotBeNull();
            result!.ProductName.Should().Be("Wireless Headphones");
        }

        [Test]
        public async Task GetProductByName_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.FindByProductName("NonExistent"))
                     .ReturnsAsync(new List<ProductEntity>());

            // Act
            var result = await _productService.GetProductByName("NonExistent");

            // Assert
            result.Should().BeNull();
        }

        //  GetProductsByCategory Tests 

        [Test]
        public async Task GetProductsByCategory_ShouldReturnProducts_WhenCategoryExists()
        {
            // Arrange
            var products = new List<ProductEntity>
            {
                new ProductEntity
                {
                    ProductName = "Product 1", Category = "Electronics",
                    ProductType = "Type1", Price = 100, Description = "Desc",
                    MerchantId = 1, Rating = new(), Review = new(),
                    Image = new List<string>(), Specification = new()
                },
                new ProductEntity
                {
                    ProductName = "Product 2", Category = "Electronics",
                    ProductType = "Type2", Price = 200, Description = "Desc",
                    MerchantId = 1, Rating = new(), Review = new(),
                    Image = new List<string>(), Specification = new()
                }
            };

            _mockRepo.Setup(r => r.FindByCategory("Electronics"))
                     .ReturnsAsync(products);

            // Act
            var result = await _productService.GetProductsByCategory("Electronics");

            // Assert
            result.Should().HaveCount(2);
            result.All(p => p.Category == "Electronics").Should().BeTrue();
        }

        //  UpdateProducts Tests 

        [Test]
        public async Task UpdateProducts_ShouldUpdateProduct_WhenCalled()
        {
            // Arrange
            var product = new ProductEntity
            {
                ProductName = "Old Name", ProductType = "Type1", Category = "Cat1",
                Price = 100, Description = "Old Desc", MerchantId = 1,
                Rating = new(), Review = new(), Image = new List<string>(), Specification = new()
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            product.ProductName = "New Name";
            product.Price = 200;

            // Act
            var result = await _productService.UpdateProducts(product);

            // Assert
            result.ProductName.Should().Be("New Name");
            result.Price.Should().Be(200);
        }

        //  DeleteProductById Tests 

        [Test]
        public async Task DeleteProductById_ShouldRemoveProduct_WhenExists()
        {
            // Arrange
            var product = new ProductEntity
            {
                ProductName = "To Delete", ProductType = "Type1", Category = "Cat1",
                Price = 100, Description = "Desc", MerchantId = 1,
                Rating = new(), Review = new(), Image = new List<string>(), Specification = new()
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            await _productService.DeleteProductById(product.ProductId);

            // Assert
            var result = await _context.Products.FindAsync(product.ProductId);
            result.Should().BeNull();
        }

        [Test]
        public async Task DeleteProductById_ShouldNotThrow_WhenProductNotFound()
        {
            // Act
            var act = async () => await _productService.DeleteProductById(999);

            // Assert
            await act.Should().NotThrowAsync();
        }
    }
}