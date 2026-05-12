using EShoppingZone.Profile.API.Entities;

namespace EShoppingZone.Profile.API.Repositories
{
    public interface IProfileRepository
    {
        Task<UserProfile?> FindByEmailAsync(string email);
        Task<UserProfile?> FindByIdAsync(int id);
        Task<IList<UserProfile>> FindAllAsync();
        Task<UserProfile> AddAsync(UserProfile profile);
        Task<UserProfile> UpdateAsync(UserProfile profile);
        Task DeleteAsync(int id);
        Task<Address> AddAddressAsync(Address address);
        Task<IList<Address>> GetAddressesByProfileIdAsync(int profileId);
    }
}