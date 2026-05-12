namespace EShoppingZone.Order.API.Entities
{
    public class OrderEntity
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public int MerchantId { get; set; }
        public decimal AmountPaid { get; set; }
        public string ModeOfPayment { get; set; } = string.Empty; // COD or EWALLET
        public string OrderStatus { get; set; } = "Placed";
        public int Quantity { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public DeliveryAddress Address { get; set; } = new();
    }
}