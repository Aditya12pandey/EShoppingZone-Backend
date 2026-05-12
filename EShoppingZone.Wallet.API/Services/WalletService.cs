using EShoppingZone.Wallet.API.Data;
using EShoppingZone.Wallet.API.Entities;
using EShoppingZone.Wallet.API.Repositories;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using EShoppingZone.Wallet.API.DTOs;

namespace EShoppingZone.Wallet.API.Services
{
    public class WalletService : IWalletService
    {
        private readonly WalletDbContext _context;
        private readonly IWalletRepository _repository;
        private readonly IConfiguration _configuration;

        public WalletService(WalletDbContext context, IWalletRepository repository, IConfiguration configuration)
        {
            _context = context;
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<IList<EWallet>> GetWallets()
        {
            return await _context.EWallets.Include(w => w.Statements).ToListAsync();
        }

        public async Task<EWallet> AddWallet(EWallet wallet)
        {
            var existing = await _context.EWallets.FindAsync(wallet.WalletId);
            if (existing != null) return existing;

            _context.EWallets.Add(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task AddMoney(int walletId, decimal amount, string remarks)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var wallet = await _context.EWallets.FindAsync(walletId);
                if (wallet == null) throw new Exception("Wallet not found.");

                wallet.CurrentBalance += amount;

                var statement = new Statement
                {
                    TransactionType = "CREDIT",
                    Amount = amount,
                    DateTime = DateTime.UtcNow,
                    OrderId = 0,
                    TransactionRemarks = remarks,
                    WalletId = walletId
                };

                await _context.Statements.AddAsync(statement);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> PayMoney(int walletId, decimal amount, string remarks, int orderId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var wallet = await _context.EWallets.FindAsync(walletId);
                if (wallet == null) throw new Exception("Wallet not found.");

                if (wallet.CurrentBalance < amount)
                    return false;

                wallet.CurrentBalance -= amount;

                var statement = new Statement
                {
                    TransactionType = "DEBIT",
                    Amount = amount,
                    DateTime = DateTime.UtcNow,
                    OrderId = orderId,
                    TransactionRemarks = remarks,
                    WalletId = walletId
                };

                await _context.Statements.AddAsync(statement);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task RefundMoney(int walletId, decimal amount, string remarks)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var wallet = await _context.EWallets.FindAsync(walletId);
                if (wallet == null) throw new Exception("Wallet not found for refund.");

                wallet.CurrentBalance += amount;

                var statement = new Statement
                {
                    TransactionType = "CREDIT",
                    Amount = amount,
                    DateTime = DateTime.UtcNow,
                    OrderId = 0,
                    TransactionRemarks = $"REFUND: {remarks}",
                    WalletId = walletId
                };

                await _context.Statements.AddAsync(statement);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<EWallet?> GetById(int id)
        {
            return await _repository.FindById(id);
        }

        public async Task<IList<Statement>> GetStatementsById(int walletId)
        {
            return await _repository.FindStatementsByWalletId(walletId);
        }

        public async Task<IList<Statement>> GetStatements()
        {
            return await _context.Statements.ToListAsync();
        }

        public async Task DeleteById(int id)
        {
            var wallet = await _context.EWallets.FindAsync(id);
            if (wallet != null)
            {
                _context.EWallets.Remove(wallet);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Dictionary<string, string>> InitiateTopUp(decimal amount, string currency)
        {
            var keyId = _configuration["Razorpay:KeyId"]!;
            var keySecret = _configuration["Razorpay:KeySecret"]!;

            var client = new RazorpayClient(keyId, keySecret);

            var options = new Dictionary<string, object>
    {
        { "amount", (int)(amount * 100) }, // paise
        { "currency", currency },
        { "receipt", $"wallet_rcpt_{Guid.NewGuid().ToString()[..8]}" },
        { "payment_capture", 1 }
    };

            var order = client.Order.Create(options);

            return new Dictionary<string, string>
    {
        { "razorpayOrderId", order["id"].ToString() },
        { "amount", order["amount"].ToString() },
        { "currency", order["currency"].ToString() },
        { "keyId", keyId }
    };
        }

        public async Task VerifyAndAddMoney(int walletId, VerifyWalletTopUpDto dto)
        {
            var keyId = _configuration["Razorpay:KeyId"]!;
            var keySecret = _configuration["Razorpay:KeySecret"]!;

            // Initialize client so Utils.verifyPaymentSignature has access to the secret
            var client = new RazorpayClient(keyId, keySecret);

            var attributes = new Dictionary<string, string>
            {
                { "razorpay_order_id", dto.RazorpayOrderId },
                { "razorpay_payment_id", dto.RazorpayPaymentId },
                { "razorpay_signature", dto.RazorpaySignature }
            };

            try
            {
                Utils.verifyPaymentSignature(attributes);
            }
            catch
            {
                throw new Exception("Payment verification failed. Invalid signature.");
            }

            // Add money to wallet after successful verification
            await AddMoney(walletId, dto.Amount, dto.Remarks);
        }
    }
}