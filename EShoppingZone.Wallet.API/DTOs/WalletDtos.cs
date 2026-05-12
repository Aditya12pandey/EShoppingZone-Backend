using System.ComponentModel.DataAnnotations;

namespace EShoppingZone.Wallet.API.DTOs
{
    public class AddMoneyDto
    {
        [Required(ErrorMessage = "Wallet ID is required.")]
        public int WalletId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(1, 100000, ErrorMessage = "Amount must be between 1 and 100000.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Remarks are required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Remarks must be between 3 and 200 characters.")]
        public string Remarks { get; set; } = string.Empty;
    }

    public class PayMoneyDto
    {
        [Required(ErrorMessage = "Wallet ID is required.")]
        public int WalletId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 9999999.99, ErrorMessage = "Amount must be between 0.01 and 9999999.99.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Remarks are required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Remarks must be between 3 and 200 characters.")]
        public string Remarks { get; set; } = string.Empty;

        [Required(ErrorMessage = "Order ID is required.")]
        public int OrderId { get; set; }
    }

    public class RefundDto
    {
        [Required(ErrorMessage = "Wallet ID is required.")]
        public int WalletId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 9999999.99, ErrorMessage = "Amount must be between 0.01 and 9999999.99.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Remarks are required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Remarks must be between 3 and 200 characters.")]
        public string Remarks { get; set; } = string.Empty;
    }

    public class InitiateWalletTopUpDto
    {
        [Required(ErrorMessage = "Amount is required.")]
        [Range(1, 100000, ErrorMessage = "Amount must be between 1 and 100000.")]
        public decimal Amount { get; set; }

        public string Currency { get; set; } = "INR";
    }

    public class VerifyWalletTopUpDto
    {
        [Required(ErrorMessage = "Razorpay Order ID is required.")]
        public string RazorpayOrderId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Razorpay Payment ID is required.")]
        public string RazorpayPaymentId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Razorpay Signature is required.")]
        public string RazorpaySignature { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required.")]
        [Range(1, 100000, ErrorMessage = "Amount must be between 1 and 100000.")]
        public decimal Amount { get; set; }

        public string Remarks { get; set; } = "Online wallet top-up";
    }
}