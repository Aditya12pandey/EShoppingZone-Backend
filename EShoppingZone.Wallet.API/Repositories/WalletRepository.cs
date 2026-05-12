using EShoppingZone.Wallet.API.Data;
using EShoppingZone.Wallet.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EShoppingZone.Wallet.API.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly WalletDbContext _context;

        public WalletRepository(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<EWallet?> FindById(int id)
        {
            return await _context.EWallets
                .Include(w => w.Statements)
                .FirstOrDefaultAsync(w => w.WalletId == id);
        }

        public async Task<IList<Statement>> FindStatementsByWalletId(int walletId)
        {
            return await _context.Statements
                .Where(s => s.WalletId == walletId)
                .ToListAsync();
        }
    }
}