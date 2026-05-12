using EShoppingZone.Cart.API.Data;
using EShoppingZone.Cart.API.Entities;
using EShoppingZone.Cart.API.Repositories;
using EShoppingZone.Cart.API.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace EShoppingZone.Cart.API.Tests
{
    [TestFixture]
    public class CartServiceTests
    {
        private Mock<ICartRepository> _mockRepo;
        private CartDbContext _context;
        private CartService _cartService;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<ICartRepository>();

            var options = new DbContextOptionsBuilder<CartDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new CartDbContext(options);

            _cartService = new CartService(_context, _mockRepo.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        //  AddCart Tests 

        [Test]
        public async Task AddCart_ShouldCreateCart_WithZeroTotal()
        {
            // Act
            var result = await _cartService.AddCart(2);

            // Assert
            result.Should().NotBeNull();
            result.CartId.Should().Be(2);
            result.TotalPrice.Should().Be(0);
        }

        [Test]
        public async Task AddCart_ShouldPersistCart_InDatabase()
        {
            // Act
            await _cartService.AddCart(5);

            // Assert
            var cart = await _context.Carts.FindAsync(5);
            cart.Should().NotBeNull();
        }

        //  GetCartById Tests 

        [Test]
        public async Task GetCartById_ShouldReturnCart_WhenExists()
        {
            // Arrange
            var cart = new CartEntity { CartId = 2, TotalPrice = 0 };
            _mockRepo.Setup(r => r.FindByCartId(2)).ReturnsAsync(cart);

            // Act
            var result = await _cartService.GetCartById(2);

            // Assert
            result.Should().NotBeNull();
            result!.CartId.Should().Be(2);
        }

        [Test]
        public async Task GetCartById_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.FindByCartId(999))
                     .ReturnsAsync((CartEntity?)null);

            // Act
            var result = await _cartService.GetCartById(999);

            // Assert
            result.Should().BeNull();
        }

        //  GetAllCarts Tests 

        [Test]
        public async Task GetAllCarts_ShouldReturnAllCarts()
        {
            // Arrange
            _context.Carts.AddRange(
                new CartEntity { CartId = 1, TotalPrice = 0 },
                new CartEntity { CartId = 2, TotalPrice = 0 },
                new CartEntity { CartId = 3, TotalPrice = 0 }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _cartService.GetAllCarts();

            // Assert
            result.Should().HaveCount(3);
        }

        //  CartTotal Tests 

        [Test]
        public void CartTotal_ShouldReturnCorrectSum()
        {
            // Arrange
            var cart = new CartEntity
            {
                CartId = 1,
                Items = new List<CartItemEntity>
                {
                    new CartItemEntity { ProductName = "Item 1", Price = 100, Quantity = 2 },
                    new CartItemEntity { ProductName = "Item 2", Price = 200, Quantity = 1 },
                    new CartItemEntity { ProductName = "Item 3", Price = 50,  Quantity = 3 }
                }
            };

            // Act
            var total = _cartService.CartTotal(cart);

            // Assert
            // 100*2 + 200*1 + 50*3 = 200 + 200 + 150 = 550
            total.Should().Be(550);
        }

        [Test]
        public void CartTotal_ShouldReturnZero_WhenCartIsEmpty()
        {
            // Arrange
            var cart = new CartEntity { CartId = 1, Items = new List<CartItemEntity>() };

            // Act
            var total = _cartService.CartTotal(cart);

            // Assert
            total.Should().Be(0);
        }

        [Test]
        public void CartTotal_ShouldHandleSingleItem()
        {
            // Arrange
            var cart = new CartEntity
            {
                CartId = 1,
                Items = new List<CartItemEntity>
                {
                    new CartItemEntity { ProductName = "Headphones", Price = 1999.99m, Quantity = 3 }
                }
            };

            // Act
            var total = _cartService.CartTotal(cart);

            // Assert
            total.Should().Be(5999.97m);
        }

        //  UpdateCart Tests 

        [Test]
        public async Task UpdateCart_ShouldRecalculateTotal_WhenItemsChange()
        {
            // Arrange
            var cart = new CartEntity
            {
                CartId = 10,
                TotalPrice = 0,
                Items = new List<CartItemEntity>
                {
                    new CartItemEntity { ProductName = "Item 1", Price = 500, Quantity = 2, CartId = 10 }
                }
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            // Act
            var result = await _cartService.UpdateCart(cart);

            // Assert
            result.TotalPrice.Should().Be(1000);
        }
    }
}