using EShoppingZone.Wallet.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EShoppingZone.Wallet.API.Data
{
    public class WalletDbContext : DbContext
    {
        public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options) { }

        public DbSet<EWallet> EWallets { get; set; }
        public DbSet<Statement> Statements { get; set; }

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

            modelBuilder.Entity<EWallet>(entity =>
            {
                entity.HasKey(w => w.WalletId);
                entity.Property(w => w.WalletId).ValueGeneratedNever();
                entity.HasMany(w => w.Statements)
                    .WithOne(s => s.EWallet)
                    .HasForeignKey(s => s.WalletId);
            });

            modelBuilder.Entity<Statement>(entity =>
            {
                entity.HasKey(s => s.StatementId);
            });
        }
    }
}