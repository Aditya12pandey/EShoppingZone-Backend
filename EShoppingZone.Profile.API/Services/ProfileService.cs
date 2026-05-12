using EShoppingZone.Profile.API.DTOs;
using EShoppingZone.Profile.API.Entities;
using EShoppingZone.Profile.API.Helpers;
using EShoppingZone.Profile.API.Repositories;
using Microsoft.AspNetCore.Identity;

namespace EShoppingZone.Profile.API.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _repo;
        private readonly JwtHelper _jwt;
        private readonly PasswordHasher<UserProfile> _hasher;

        public ProfileService(IProfileRepository repo, JwtHelper jwt)
        {
            _repo = repo;
            _jwt = jwt;
            _hasher = new PasswordHasher<UserProfile>();
        }

        public async Task<UserProfile> RegisterCustomerAsync(RegisterCustomerDto dto)
        {
            var existing = await _repo.FindByEmailAsync(dto.EmailId);
            if (existing != null)
                throw new InvalidOperationException("Email already registered.");

            var user = new UserProfile
            {
                FullName = dto.FullName,
                EmailId = dto.EmailId,
                MobileNumber = dto.MobileNumber,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                Role = "CUSTOMER"
            };
            user.Password = _hasher.HashPassword(user, dto.Password);
            return await _repo.AddAsync(user);
        }

        public async Task<UserProfile> RegisterMerchantAsync(RegisterMerchantDto dto)
        {
            var existing = await _repo.FindByEmailAsync(dto.EmailId);
            if (existing != null)
                throw new InvalidOperationException("Email already registered.");

            var user = new UserProfile
            {
                FullName = dto.FullName,
                EmailId = dto.EmailId,
                MobileNumber = dto.MobileNumber,
                About = dto.About,
                Role = "MERCHANT"
            };
            user.Password = _hasher.HashPassword(user, dto.Password);
            return await _repo.AddAsync(user);
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _repo.FindByEmailAsync(dto.EmailId);
            if (user == null) return null;

            var result = _hasher.VerifyHashedPassword(user, user.Password, dto.Password);
            if (result == PasswordVerificationResult.Failed) return null;

            return new LoginResponseDto
            {
                Token = _jwt.BuildToken(user),
                Role = user.Role,
                ProfileId = user.ProfileId,
                FullName = user.FullName
            };
        }

        public async Task<UserProfile?> GetByIdAsync(int id)
            => await _repo.FindByIdAsync(id);

        public async Task<IList<UserProfile>> GetAllAsync()
            => await _repo.FindAllAsync();

        public async Task<UserProfile> UpdateAsync(int id, ProfileUpdateDto dto)
        {
            var user = await _repo.FindByIdAsync(id)
                ?? throw new KeyNotFoundException("Profile not found.");

            if (dto.FullName != null) user.FullName = dto.FullName;
            if (dto.MobileNumber.HasValue) user.MobileNumber = dto.MobileNumber.Value;
            if (dto.About != null) user.About = dto.About;
            if (dto.DateOfBirth.HasValue) user.DateOfBirth = dto.DateOfBirth;
            if (dto.Gender != null) user.Gender = dto.Gender;
            if (dto.Image != null) user.Image = dto.Image;

            return await _repo.UpdateAsync(user);
        }

        public async Task DeleteAsync(int id)
            => await _repo.DeleteAsync(id);

        public async Task<Address> AddAddressAsync(AddressDto dto)
        {
            var address = new Address
            {
                HouseNumber = dto.HouseNumber,
                StreetName = dto.StreetName,
                ColonyName = dto.ColonyName,
                City = dto.City,
                State = dto.State,
                Pincode = (dto.Pincode),
                ProfileId = dto.ProfileId
            };
            return await _repo.AddAddressAsync(address);
        }

        public async Task<IList<Address>> GetAddressesAsync(int profileId)
            => await _repo.GetAddressesByProfileIdAsync(profileId);
    }
}