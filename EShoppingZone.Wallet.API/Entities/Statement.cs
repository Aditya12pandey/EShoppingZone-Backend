using System.Text.Json.Serialization;

namespace EShoppingZone.Wallet.API.Entities
{
    public class Statement
    {
        public int StatementId { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DateTime { get; set; }
        public int OrderId { get; set; }
        public string TransactionRemarks { get; set; } = string.Empty;
        public int WalletId { get; set; }

        [JsonIgnore]
        public EWallet? EWallet { get; set; }
    }
}