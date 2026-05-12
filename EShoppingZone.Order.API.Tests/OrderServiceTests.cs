using EShoppingZone.Order.API.Data;
using EShoppingZone.Order.API.DTOs;
using EShoppingZone.Order.API.Entities;
using EShoppingZone.Order.API.Repositories;
using EShoppingZone.Order.API.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace EShoppingZone.Order.API.Tests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private Mock<IOrderRepository> _mockRepo;
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private OrderDbContext _context;
        private OrderService _orderService;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IOrderRepository>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();

            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            _context = new OrderDbContext(options);

            _orderService = new OrderService(_context, _mockRepo.Object, _mockHttpClientFactory.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        //  PlaceOrder COD Tests 

        [Test]
        public async Task PlaceOrder_ShouldCreateOrder_WhenCOD()
        {
            // Arrange
            var dto = new PlaceOrderDto
            {
                CustomerId = 2,
                MerchantId = 3,
                AmountPaid = 1999.99m,
                ModeOfPayment = "COD",
                Quantity = 1,
                ProductName = "Wireless Headphones",
                ProductId = 1,
                Address = new DeliveryAddress
                {
                    FullName = "Test Customer",
                    MobileNumber = "9999999999",
                    FlatNumber = "101",
                    City = "Delhi",
                    State = "Delhi",
                    Pincode = "110001"
                }
            };

            // Act
            var result = await _orderService.PlaceOrder(dto);

            // Assert
            result.Should().NotBeNull();
            result.CustomerId.Should().Be(2);
            result.ModeOfPayment.Should().Be("COD");
            result.OrderStatus.Should().Be("Placed");
            result.AmountPaid.Should().Be(1999.99m);
        }

        [Test]
        public async Task PlaceOrder_ShouldPersistOrder_InDatabase()
        {
            // Arrange
            var dto = new PlaceOrderDto
            {
                CustomerId = 2,
                MerchantId = 3,
                AmountPaid = 500,
                ModeOfPayment = "COD",
                Quantity = 1,
                ProductName = "Test Product",
                ProductId = 1,
                Address = new DeliveryAddress
                {
                    FullName = "Customer",
                    MobileNumber = "9999999999",
                    FlatNumber = "101",
                    City = "Delhi",
                    State = "Delhi",
                    Pincode = "110001"
                }
            };

            // Act
            await _orderService.PlaceOrder(dto);

            // Assert
            var order = await _context.Orders.FirstOrDefaultAsync();
            order.Should().NotBeNull();
            order!.ProductName.Should().Be("Test Product");
        }

        //  GetAllOrders Tests 

        [Test]
        public async Task GetAllOrders_ShouldReturnAllOrders()
        {
            // Arrange
            _context.Orders.AddRange(
                new OrderEntity
                {
                    CustomerId = 1, MerchantId = 2, AmountPaid = 100,
                    ModeOfPayment = "COD", OrderStatus = "Placed",
                    Quantity = 1, ProductName = "P1", ProductId = 1,
                    OrderDate = DateTime.UtcNow,
                    Address = new DeliveryAddress
                    {
                        FullName = "Test", MobileNumber = "9999999999",
                        FlatNumber = "1", City = "Delhi", State = "Delhi", Pincode = "110001"
                    }
                },
                new OrderEntity
                {
                    CustomerId = 2, MerchantId = 3, AmountPaid = 200,
                    ModeOfPayment = "COD", OrderStatus = "Placed",
                    Quantity = 2, ProductName = "P2", ProductId = 2,
                    OrderDate = DateTime.UtcNow,
                    Address = new DeliveryAddress
                    {
                        FullName = "Test2", MobileNumber = "8888888888",
                        FlatNumber = "2", City = "Mumbai", State = "Maharashtra", Pincode = "400001"
                    }
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderService.GetAllOrders();

            // Assert
            result.Should().HaveCount(2);
        }

        //  GetOrderByCustomerId Tests 

        [Test]
        public async Task GetOrderByCustomerId_ShouldReturnOrders_WhenExists()
        {
            // Arrange
            var orders = new List<OrderEntity>
            {
                new OrderEntity
                {
                    OrderId = 1, CustomerId = 2, MerchantId = 3,
                    AmountPaid = 100, ModeOfPayment = "COD",
                    OrderStatus = "Placed", Quantity = 1,
                    ProductName = "P1", ProductId = 1,
                    OrderDate = DateTime.UtcNow,
                    Address = new DeliveryAddress
                    {
                        FullName = "Test", MobileNumber = "9999999999",
                        FlatNumber = "1", City = "Delhi", State = "Delhi", Pincode = "110001"
                    }
                }
            };

            _mockRepo.Setup(r => r.FindByCustomerId(2)).ReturnsAsync(orders);

            // Act
            var result = await _orderService.GetOrderByCustomerId(2);

            // Assert
            result.Should().HaveCount(1);
            result.First().CustomerId.Should().Be(2);
        }

        //  ChangeStatus Tests 

        [Test]
        public async Task ChangeStatus_ShouldUpdateStatus_WhenOrderExists()
        {
            // Arrange
            var order = new OrderEntity
            {
                CustomerId = 1, MerchantId = 2, AmountPaid = 100,
                ModeOfPayment = "COD", OrderStatus = "Placed",
                Quantity = 1, ProductName = "P1", ProductId = 1,
                OrderDate = DateTime.UtcNow,
                Address = new DeliveryAddress
                {
                    FullName = "Test", MobileNumber = "9999999999",
                    FlatNumber = "1", City = "Delhi", State = "Delhi", Pincode = "110001"
                }
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderService.ChangeStatus(order.OrderId, "Shipped");

            // Assert
            result.Should().BeTrue();
            var updated = await _context.Orders.FindAsync(order.OrderId);
            updated!.OrderStatus.Should().Be("Shipped");
        }

        [Test]
        public async Task ChangeStatus_ShouldReturnFalse_WhenOrderNotFound()
        {
            // Act
            var result = await _orderService.ChangeStatus(999, "Shipped");

            // Assert
            result.Should().BeFalse();
        }

        //  DeleteOrder Tests 

        [Test]
        public async Task DeleteOrder_ShouldRemoveOrder_WhenExists()
        {
            // Arrange
            var order = new OrderEntity
            {
                CustomerId = 1, MerchantId = 2, AmountPaid = 100,
                ModeOfPayment = "COD", OrderStatus = "Placed",
                Quantity = 1, ProductName = "P1", ProductId = 1,
                OrderDate = DateTime.UtcNow,
                Address = new DeliveryAddress
                {
                    FullName = "Test", MobileNumber = "9999999999",
                    FlatNumber = "1", City = "Delhi", State = "Delhi", Pincode = "110001"
                }
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Act
            await _orderService.DeleteOrder(order.OrderId);

            // Assert
            var result = await _context.Orders.FindAsync(order.OrderId);
            result.Should().BeNull();
        }
    }
}