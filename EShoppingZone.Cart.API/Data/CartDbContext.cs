using EShoppingZone.Cart.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EShoppingZone.Cart.API.Data
{
    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> options) : base(options) { }

        public DbSet<CartEntity> Carts { get; set; }
        public DbSet<CartItemEntity> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CartEntity>(entity =>
            {
                entity.HasKey(c => c.CartId);
                entity.Property(c => c.CartId).ValueGeneratedNever();
                entity.HasMany(c => c.Items)
                    .WithOne(i => i.Cart)
                    .HasForeignKey(i => i.CartId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CartItemEntity>(entity =>
            {
                entity.HasKey(i => i.CartItemId);
            });
        }
    }
}