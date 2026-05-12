using EShoppingZone.Wallet.API.Entities;

namespace EShoppingZone.Wallet.API.Repositories
{
    public interface IWalletRepository
    {
        Task<EWallet?> FindById(int id);
        Task<IList<Statement>> FindStatementsByWalletId(int walletId);
    }
}