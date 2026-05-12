using EShoppingZone.Profile.API.Data;
using EShoppingZone.Profile.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EShoppingZone.Profile.API.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly ProfileDbContext _context;

        public ProfileRepository(ProfileDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile?> FindByEmailAsync(string email)
            => await _context.UserProfiles
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.EmailId == email);

        public async Task<UserProfile?> FindByIdAsync(int id)
            => await _context.UserProfiles
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.ProfileId == id);

        public async Task<IList<UserProfile>> FindAllAsync()
            => await _context.UserProfiles
                .Include(u => u.Addresses)
                .ToListAsync();

        public async Task<UserProfile> AddAsync(UserProfile profile)
        {
            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        public async Task<UserProfile> UpdateAsync(UserProfile profile)
        {
            _context.UserProfiles.Update(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        public async Task DeleteAsync(int id)
        {
            var profile = await FindByIdAsync(id);
            if (profile != null)
            {
                _context.UserProfiles.Remove(profile);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Address> AddAddressAsync(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<IList<Address>> GetAddressesByProfileIdAsync(int profileId)
            => await _context.Addresses
                .Where(a => a.ProfileId == profileId)
                .ToListAsync();
    }
}