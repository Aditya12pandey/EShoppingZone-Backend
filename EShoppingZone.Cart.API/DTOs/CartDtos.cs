using System.ComponentModel.DataAnnotations;

namespace EShoppingZone.Cart.API.DTOs
{
    public class AddToCartDto
    {
        [Required(ErrorMessage = "Product ID is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Product name must be between 3 and 200 characters.")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 9999999.99, ErrorMessage = "Price must be between 0.01 and 9999999.99.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
        public int Quantity { get; set; }
        
        [Required(ErrorMessage = "Merchant ID is required.")]
        public int MerchantId { get; set; }
    }
}