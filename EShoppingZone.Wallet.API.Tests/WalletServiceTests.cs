using EShoppingZone.Wallet.API.Data;
using EShoppingZone.Wallet.API.Entities;
using EShoppingZone.Wallet.API.Repositories;
using EShoppingZone.Wallet.API.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace EShoppingZone.Wallet.API.Tests
{
    [TestFixture]
    public class WalletServiceTests
    {
        private Mock<IWalletRepository> _mockRepo;
        private Mock<IConfiguration> _mockConfiguration;
        private WalletDbContext _context;
        private WalletService _walletService;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IWalletRepository>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Provide Razorpay keys so InitiateTopUp / VerifyAndAddMoney don't throw on config lookup
            _mockConfiguration.Setup(c => c["Razorpay:KeyId"]).Returns("test_key_id");
            _mockConfiguration.Setup(c => c["Razorpay:KeySecret"]).Returns("test_key_secret");

            var options = new DbContextOptionsBuilder<WalletDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            _context = new WalletDbContext(options);

            _walletService = new WalletService(_context, _mockRepo.Object, _mockConfiguration.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        // AddWallet Tests 

        [Test]
        public async Task AddWallet_ShouldCreateWallet_WithZeroBalance()
        {
            // Arrange
            var wallet = new EWallet { WalletId = 2, CurrentBalance = 0 };

            // Act
            var result = await _walletService.AddWallet(wallet);

            // Assert
            result.Should().NotBeNull();
            result.WalletId.Should().Be(2);
            result.CurrentBalance.Should().Be(0);
        }

        [Test]
        public async Task AddWallet_ShouldPersistWallet_InDatabase()
        {
            // Arrange
            var wallet = new EWallet { WalletId = 3, CurrentBalance = 0 };

            // Act
            await _walletService.AddWallet(wallet);

            // Assert
            var result = await _context.EWallets.FindAsync(3);
            result.Should().NotBeNull();
        }

        //  AddMoney Tests 

        [Test]
        public async Task AddMoney_ShouldIncrementBalance_WhenCalled()
        {
            // Arrange
            var wallet = new EWallet { WalletId = 2, CurrentBalance = 1000 };
            _context.EWallets.Add(wallet);
            await _context.SaveChangesAsync();

            // Act
            await _walletService.AddMoney(2, 500, "Test deposit");

            // Assert
            var updated = await _context.EWallets.FindAsync(2);
            updated!.CurrentBalance.Should().Be(1500);
        }

        [Test]
        public async Task AddMoney_ShouldCreateCreditStatement_WhenCalled()
        {
            // Arrange
            var wallet = new EWallet { WalletId = 2, CurrentBalance = 0 };
            _context.EWallets.Add(wallet);
            await _context.SaveChangesAsync();

            // Act
            await _walletService.AddMoney(2, 1000, "Initial deposit");

            // Assert
            var statement = await _context.Statements.FirstOrDefaultAsync();
            statement.Should().NotBeNull();
            statement!.TransactionType.Should().Be("CREDIT");
            statement.Amount.Should().Be(1000);
            statement.WalletId.Should().Be(2);
        }

        [Test]
        public async Task AddMoney_ShouldThrow_WhenWalletNotFound()
        {
            // Act
            var act = async () => await _walletService.AddMoney(999, 500, "Test");

            // Assert
            await act.Should().ThrowAsync<Exception>()
                     .WithMessage("Wallet not found.");
        }

        //  PayMoney Tests 

        [Test]
        public async Task PayMoney_ShouldDeductBalance_WhenSufficientFunds()
        {
            // Arrange
            var wallet = new EWallet { WalletId = 2, CurrentBalance = 5000 };
            _context.EWallets.Add(wallet);
            await _context.SaveChangesAsync();

            // Act
            var result = await _walletService.PayMoney(2, 1999.99m, "Order payment", 1);

            // Assert
            result.Should().BeTrue();
            var updated = await _context.EWallets.FindAsync(2);
            updated!.CurrentBalance.Should().Be(3000.01m);
        }

        [Test]
        public async Task PayMoney_ShouldReturnFalse_WhenInsufficientFunds()
        {
            // Arrange
            var wallet = new EWallet { WalletId = 2, CurrentBalance = 100 };
            _context.EWallets.Add(wallet);
            await _context.SaveChangesAsync();

            // Act
            var result = await _walletService.PayMoney(2, 999999, "Order payment", 1);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public async Task PayMoney_ShouldCreateDebitStatement_WhenSuccessful()
        {
            // Arrange
            var wallet = new EWallet { WalletId = 2, CurrentBalance = 5000 };
            _context.EWallets.Add(wallet);
            await _context.SaveChangesAsync();

            // Act
            await _walletService.PayMoney(2, 1000, "Order payment", 1);

            // Assert
            var statement = await _context.Statements.FirstOrDefaultAsync();
            statement.Should().NotBeNull();
            statement!.TransactionType.Should().Be("DEBIT");
            statement.Amount.Should().Be(1000);
        }

        [Test]
        public async Task PayMoney_ShouldNotDeductBalance_WhenInsufficientFunds()
        {
            // Arrange
            var wallet = new EWallet { WalletId = 2, CurrentBalance = 100 };
            _context.EWallets.Add(wallet);
            await _context.SaveChangesAsync();

            // Act
            await _walletService.PayMoney(2, 999999, "Order payment", 1);

            // Assert — balance should remain unchanged
            var updated = await _context.EWallets.FindAsync(2);
            updated!.CurrentBalance.Should().Be(100);
        }

        //  RefundMoney Tests 

        [Test]
        public async Task RefundMoney_ShouldIncrementBalance_WhenCalled()
        {
            // Arrange
            var wallet = new EWallet { WalletId = 2, CurrentBalance = 1000 };
            _context.EWallets.Add(wallet);
            await _context.SaveChangesAsync();

            // Act
            await _walletService.RefundMoney(2, 500, "Order failed refund");

            // Assert
            var updated = await _context.EWallets.FindAsync(2);
            updated!.CurrentBalance.Should().Be(1500);
        }

        [Test]
        public async Task RefundMoney_ShouldCreateCreditStatement_WithRefundRemarks()
        {
            // Arrange
            var wallet = new EWallet { WalletId = 2, CurrentBalance = 0 };
            _context.EWallets.Add(wallet);
            await _context.SaveChangesAsync();

            // Act
            await _walletService.RefundMoney(2, 500, "Order failed");

            // Assert
            var statement = await _context.Statements.FirstOrDefaultAsync();
            statement.Should().NotBeNull();
            statement!.TransactionType.Should().Be("CREDIT");
            statement.TransactionRemarks.Should().Contain("REFUND");
        }

        [Test]
        public async Task RefundMoney_ShouldThrow_WhenWalletNotFound()
        {
            // Act
            var act = async () => await _walletService.RefundMoney(999, 500, "Refund");

            // Assert
            await act.Should().ThrowAsync<Exception>()
                     .WithMessage("Wallet not found for refund.");
        }

        //  GetById Tests 

        [Test]
        public async Task GetById_ShouldReturnWallet_WhenExists()
        {
            // Arrange
            var wallet = new EWallet { WalletId = 2, CurrentBalance = 1000 };
            _mockRepo.Setup(r => r.FindById(2)).ReturnsAsync(wallet);

            // Act
            var result = await _walletService.GetById(2);

            // Assert
            result.Should().NotBeNull();
            result!.WalletId.Should().Be(2);
            result.CurrentBalance.Should().Be(1000);
        }

        [Test]
        public async Task GetById_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.FindById(999)).ReturnsAsync((EWallet?)null);

            // Act
            var result = await _walletService.GetById(999);

            // Assert
            result.Should().BeNull();
        }

        //  GetStatementsById Tests 

        [Test]
        public async Task GetStatementsById_ShouldReturnStatements_WhenExists()
        {
            // Arrange
            var statements = new List<Statement>
            {
                new Statement { StatementId = 1, WalletId = 2, Amount = 1000, TransactionType = "CREDIT" },
                new Statement { StatementId = 2, WalletId = 2, Amount = 500,  TransactionType = "DEBIT" }
            };
            _mockRepo.Setup(r => r.FindStatementsByWalletId(2)).ReturnsAsync(statements);

            // Act
            var result = await _walletService.GetStatementsById(2);

            // Assert
            result.Should().HaveCount(2);
        }

        //  DeleteById Tests 

        [Test]
        public async Task DeleteById_ShouldRemoveWallet_WhenExists()
        {
            // Arrange
            var wallet = new EWallet { WalletId = 2, CurrentBalance = 0 };
            _context.EWallets.Add(wallet);
            await _context.SaveChangesAsync();

            // Act
            await _walletService.DeleteById(2);

            // Assert
            var result = await _context.EWallets.FindAsync(2);
            result.Should().BeNull();
        }
    }
}