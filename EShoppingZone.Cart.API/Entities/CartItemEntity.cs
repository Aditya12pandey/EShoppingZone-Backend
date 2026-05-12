using System.Text.Json.Serialization;

namespace EShoppingZone.Cart.API.Entities
{
    public class CartItemEntity
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int MerchantId { get; set; }
        public int CartId { get; set; }

        [JsonIgnore]
        public CartEntity? Cart { get; set; }
    }
}