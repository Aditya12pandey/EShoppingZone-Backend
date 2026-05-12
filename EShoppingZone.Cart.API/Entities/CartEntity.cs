namespace EShoppingZone.Cart.API.Entities
{
    public class CartEntity
    {
        public int CartId { get; set; } // equals UserId
        public decimal TotalPrice { get; set; }
        public IList<CartItemEntity> Items { get; set; } = new List<CartItemEntity>();
    }
}