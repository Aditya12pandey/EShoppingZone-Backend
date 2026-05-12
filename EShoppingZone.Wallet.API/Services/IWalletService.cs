using EShoppingZone.Wallet.API.Entities;
using EShoppingZone.Wallet.API.DTOs;

namespace EShoppingZone.Wallet.API.Services
{
    public interface IWalletService
    {
        Task<IList<EWallet>> GetWallets();
        Task<EWallet> AddWallet(EWallet wallet);
        Task AddMoney(int walletId, decimal amount, string remarks);
        Task<bool> PayMoney(int walletId, decimal amount, string remarks, int orderId);
        Task RefundMoney(int walletId, decimal amount, string remarks);
        Task<EWallet?> GetById(int id);
        Task<IList<Statement>> GetStatementsById(int walletId);
        Task<IList<Statement>> GetStatements();
        Task DeleteById(int id);
        Task<Dictionary<string, string>> InitiateTopUp(decimal amount, string currency);
        Task VerifyAndAddMoney(int walletId, VerifyWalletTopUpDto dto);
    }
}