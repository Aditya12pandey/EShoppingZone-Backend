using System.ComponentModel.DataAnnotations;
using EShoppingZone.Order.API.Entities;

namespace EShoppingZone.Order.API.DTOs
{
    public class PlaceOrderDto
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Merchant ID is required.")]
        public int MerchantId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 9999999.99, ErrorMessage = "Amount must be between 0.01 and 9999999.99.")]
        public decimal AmountPaid { get; set; }

        [Required(ErrorMessage = "Mode of payment is required.")]
        [RegularExpression(@"^(COD|EWALLET|ONLINE)$", ErrorMessage = "Mode of payment must be COD, EWALLET or ONLINE.")]
        public string ModeOfPayment { get; set; } = string.Empty;
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Product name must be between 3 and 200 characters.")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product ID is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Delivery address is required.")]
        public DeliveryAddress Address { get; set; } = new();

        [System.Text.Json.Serialization.JsonIgnore]
        public string? BearerToken { get; set; }
    }

    public class ChangeStatusDto
    {
        [Required(ErrorMessage = "Order ID is required.")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression(@"(?i)^(Placed|Shipped|Delivered|Cancelled)$",
            ErrorMessage = "Status must be Placed, Shipped, Delivered or Cancelled.")]
        public string Status { get; set; } = string.Empty;
    }

    public class InitiatePaymentDto
    {
        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 9999999.99, ErrorMessage = "Amount must be between 0.01 and 9999999.99.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Currency is required.")]
        public string Currency { get; set; } = "INR";
    }

    public class VerifyPaymentDto
    {
        [Required(ErrorMessage = "Razorpay Order ID is required.")]
        public string RazorpayOrderId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Razorpay Payment ID is required.")]
        public string RazorpayPaymentId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Razorpay Signature is required.")]
        public string RazorpaySignature { get; set; } = string.Empty;

        // Order details to place after payment verified
        public int MerchantId { get; set; }
        public decimal AmountPaid { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public DeliveryAddress Address { get; set; } = new();
    }
}