using EShoppingZone.Profile.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EShoppingZone.Profile.API.Data
{
    public class ProfileDbContext : DbContext
    {
        public ProfileDbContext(DbContextOptions<ProfileDbContext> options) : base(options) { }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserProfile>()
                .HasMany(u => u.Addresses)
                .WithOne(a => a.UserProfile)
                .HasForeignKey(a => a.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserProfile>()
                .HasIndex(u => u.EmailId)
                .IsUnique();

            var hasher = new PasswordHasher<UserProfile>();
            var admin = new UserProfile
            {
                ProfileId = 1,
                FullName = "Platform Admin",
                EmailId = "admin@eshoppingzone.com",
                MobileNumber = 9000000000,
                Role = "ADMIN",
                Password = "placeholder"
            };
            admin.Password = hasher.HashPassword(admin, "Admin@123");

            modelBuilder.Entity<UserProfile>().HasData(admin);
        }
    }
}