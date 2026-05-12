namespace EShoppingZone.Wallet.API.Entities
{
    public class EWallet
    {
        public int WalletId { get; set; }
        public decimal CurrentBalance { get; set; }
        public IList<Statement> Statements { get; set; } = new List<Statement>();
    }
}