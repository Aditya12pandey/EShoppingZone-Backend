using EShoppingZone.Order.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EShoppingZone.Order.API.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<OrderEntity> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderEntity>(entity =>
            {
                entity.HasKey(o => o.OrderId);
                entity.OwnsOne(o => o.Address);
            });
        }
    }
}