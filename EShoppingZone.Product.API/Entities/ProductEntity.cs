namespace EShoppingZone.Product.API.Entities
{
    public class ProductEntity
    {
        public int ProductId { get; set; }
        public string ProductType { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public Dictionary<int, double> Rating { get; set; } = new();
        public Dictionary<int, string> Review { get; set; } = new();
        public IList<string> Image { get; set; } = new List<string>();
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, string> Specification { get; set; } = new();
        public int MerchantId { get; set; }
    }
}