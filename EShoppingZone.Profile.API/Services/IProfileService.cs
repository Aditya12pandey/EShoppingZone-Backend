using EShoppingZone.Profile.API.DTOs;
using EShoppingZone.Profile.API.Entities;

namespace EShoppingZone.Profile.API.Services
{
    public interface IProfileService
    {
        Task<UserProfile> RegisterCustomerAsync(RegisterCustomerDto dto);
        Task<UserProfile> RegisterMerchantAsync(RegisterMerchantDto dto);
        Task<LoginResponseDto?> LoginAsync(LoginDto dto);
        Task<UserProfile?> GetByIdAsync(int id);
        Task<IList<UserProfile>> GetAllAsync();
        Task<UserProfile> UpdateAsync(int id, ProfileUpdateDto dto);
        Task DeleteAsync(int id);
        Task<Address> AddAddressAsync(AddressDto dto);
        Task<IList<Address>> GetAddressesAsync(int profileId);
    }
}