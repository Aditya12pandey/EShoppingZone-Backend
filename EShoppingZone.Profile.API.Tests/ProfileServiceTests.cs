using EShoppingZone.Profile.API.DTOs;
using EShoppingZone.Profile.API.Entities;
using EShoppingZone.Profile.API.Helpers;
using EShoppingZone.Profile.API.Repositories;
using EShoppingZone.Profile.API.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace EShoppingZone.Profile.API.Tests
{
    [TestFixture]
    public class ProfileServiceTests
    {
        private Mock<IProfileRepository> _mockRepo;
        private Mock<IConfiguration> _mockConfig;
        private JwtHelper _jwtHelper;
        private ProfileService _profileService;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IProfileRepository>();

            // Setup JWT config
            var inMemorySettings = new Dictionary<string, string>
            {
                { "Jwt:Key", "EShoppingZone_SuperSecretKey_2026_DoNotShare_MinLength32Chars!" },
                { "Jwt:Issuer", "EShoppingZone" },
                { "Jwt:Audience", "EShoppingZoneClients" },
                { "Jwt:ExpiryHours", "24" }
            };
            _mockConfig = new Mock<IConfiguration>();
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            _jwtHelper = new JwtHelper(config);
            _profileService = new ProfileService(_mockRepo.Object, _jwtHelper);
        }

        //  RegisterCustomer Tests 

        [Test]
        public async Task RegisterCustomerAsync_ShouldReturnUser_WhenEmailNotTaken()
        {
            // Arrange
            var dto = new RegisterCustomerDto
            {
                FullName = "Test Customer",
                EmailId = "test@test.com",
                MobileNumber = 9999999999,
                Password = "Test@1234",
                Gender = "Male",
                DateOfBirth = new DateTime(2000, 1, 1)
            };

            _mockRepo.Setup(r => r.FindByEmailAsync(dto.EmailId))
                     .ReturnsAsync((UserProfile?)null);

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<UserProfile>()))
                     .ReturnsAsync((UserProfile u) => u);

            // Act
            var result = await _profileService.RegisterCustomerAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.FullName.Should().Be("Test Customer");
            result.Role.Should().Be("CUSTOMER");
            result.EmailId.Should().Be("test@test.com");
        }

        [Test]
        public async Task RegisterCustomerAsync_ShouldThrow_WhenEmailAlreadyExists()
        {
            // Arrange
            var dto = new RegisterCustomerDto
            {
                EmailId = "existing@test.com",
                FullName = "Test",
                Password = "Test@1234",
                MobileNumber = 9999999999,
                Gender = "Male",
                DateOfBirth = new DateTime(2000, 1, 1)
            };

            _mockRepo.Setup(r => r.FindByEmailAsync(dto.EmailId))
                     .ReturnsAsync(new UserProfile { EmailId = dto.EmailId });

            // Act
            var act = async () => await _profileService.RegisterCustomerAsync(dto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Email already registered.");
        }

        //  RegisterMerchant Tests 

        [Test]
        public async Task RegisterMerchantAsync_ShouldReturnMerchant_WhenEmailNotTaken()
        {
            // Arrange
            var dto = new RegisterMerchantDto
            {
                FullName = "Test Merchant",
                EmailId = "merchant@test.com",
                MobileNumber = 8888888888,
                Password = "Merchant@123",
                Gender = "Male",
                About = "Test merchant"
            };

            _mockRepo.Setup(r => r.FindByEmailAsync(dto.EmailId))
                     .ReturnsAsync((UserProfile?)null);

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<UserProfile>()))
                     .ReturnsAsync((UserProfile u) => u);

            // Act
            var result = await _profileService.RegisterMerchantAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Role.Should().Be("MERCHANT");
            result.FullName.Should().Be("Test Merchant");
        }

        [Test]
        public async Task RegisterMerchantAsync_ShouldThrow_WhenEmailAlreadyExists()
        {
            // Arrange
            var dto = new RegisterMerchantDto
            {
                EmailId = "existing@test.com",
                FullName = "Test",
                Password = "Test@1234",
                MobileNumber = 9999999999
            };

            _mockRepo.Setup(r => r.FindByEmailAsync(dto.EmailId))
                     .ReturnsAsync(new UserProfile { EmailId = dto.EmailId });

            // Act
            var act = async () => await _profileService.RegisterMerchantAsync(dto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Email already registered.");
        }

        //  Login Tests 

        [Test]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var dto = new LoginDto
            {
                EmailId = "test@test.com",
                Password = "Test@1234"
            };

            var user = new UserProfile
            {
                ProfileId = 1,
                EmailId = "test@test.com",
                FullName = "Test User",
                Role = "CUSTOMER"
            };

            // Hash the password the same way ProfileService does
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<UserProfile>();
            user.Password = hasher.HashPassword(user, "Test@1234");

            _mockRepo.Setup(r => r.FindByEmailAsync(dto.EmailId))
                     .ReturnsAsync(user);

            // Act
            var result = await _profileService.LoginAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.Role.Should().Be("CUSTOMER");
        }

        [Test]
        public async Task LoginAsync_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange
            var dto = new LoginDto { EmailId = "notfound@test.com", Password = "Test@1234" };

            _mockRepo.Setup(r => r.FindByEmailAsync(dto.EmailId))
                     .ReturnsAsync((UserProfile?)null);

            // Act
            var result = await _profileService.LoginAsync(dto);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsWrong()
        {
            // Arrange
            var dto = new LoginDto { EmailId = "test@test.com", Password = "WrongPassword" };

            var user = new UserProfile { EmailId = "test@test.com", Role = "CUSTOMER" };
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<UserProfile>();
            user.Password = hasher.HashPassword(user, "CorrectPassword@1");

            _mockRepo.Setup(r => r.FindByEmailAsync(dto.EmailId))
                     .ReturnsAsync(user);

            // Act
            var result = await _profileService.LoginAsync(dto);

            // Assert
            result.Should().BeNull();
        }

        //  GetById Tests 

        [Test]
        public async Task GetByIdAsync_ShouldReturnUser_WhenExists()
        {
            // Arrange
            var user = new UserProfile { ProfileId = 1, FullName = "Test User" };
            _mockRepo.Setup(r => r.FindByIdAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _profileService.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.ProfileId.Should().Be(1);
            result.FullName.Should().Be("Test User");
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.FindByIdAsync(999))
                     .ReturnsAsync((UserProfile?)null);

            // Act
            var result = await _profileService.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        //  GetAll Tests 

        [Test]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<UserProfile>
            {
                new UserProfile { ProfileId = 1, FullName = "User One" },
                new UserProfile { ProfileId = 2, FullName = "User Two" },
                new UserProfile { ProfileId = 3, FullName = "User Three" }
            };
            _mockRepo.Setup(r => r.FindAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _profileService.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
        }

        //  Delete Tests 

        [Test]
        public async Task DeleteAsync_ShouldCallRepository_WhenCalled()
        {
            // Arrange
            _mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            await _profileService.DeleteAsync(1);

            // Assert
            _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
        }
    }
}