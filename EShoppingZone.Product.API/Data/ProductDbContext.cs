using EShoppingZone.Product.API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EShoppingZone.Product.API.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

        public DbSet<ProductEntity> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Force lowercase for PostgreSQL compatibility
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName()?.ToLower());
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName().ToLower());
                }
            }

            modelBuilder.Entity<ProductEntity>(entity =>
            {
                entity.HasKey(p => p.ProductId);

                entity.Property(p => p.Rating)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<Dictionary<int, double>>(v, (JsonSerializerOptions?)null) ?? new()
                    );

                entity.Property(p => p.Review)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<Dictionary<int, string>>(v, (JsonSerializerOptions?)null) ?? new()
                    );

                entity.Property(p => p.Image)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<IList<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
                    );

                entity.Property(p => p.Specification)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new()
                    );
            });
        }
    }
}