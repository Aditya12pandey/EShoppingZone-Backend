using System.ComponentModel.DataAnnotations;

namespace EShoppingZone.Product.API.DTOs
{
    public class AddProductDto
    {
        [Required(ErrorMessage = "Product type is required.")]
        [RegularExpression(@"^[a-zA-Z0-9\s\&\-\/\.]+$", ErrorMessage = "Product type can only contain letters, numbers, spaces, and characters like &-/.")]
        public string ProductType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Product name must be between 3 and 200 characters.")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        [RegularExpression(@"^[a-zA-Z0-9\s\&\-\/\.]+$", ErrorMessage = "Category can only contain letters, numbers, spaces, and characters like &-/.")]
        public string Category { get; set; } = string.Empty;

        public IList<string> Image { get; set; } = new List<string>();

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 9999999.99, ErrorMessage = "Price must be between 0.01 and 9999999.99.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters.")]
        public string Description { get; set; } = string.Empty;

        public Dictionary<string, string> Specification { get; set; } = new();
    }

    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Product ID is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product type is required.")]
        [RegularExpression(@"^[a-zA-Z0-9\s\&\-\/\.]+$", ErrorMessage = "Product type can only contain letters, numbers, spaces, and characters like &-/.")]
        public string ProductType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Product name must be between 3 and 200 characters.")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        [RegularExpression(@"^[a-zA-Z0-9\s\&\-\/\.]+$", ErrorMessage = "Category can only contain letters, numbers, spaces, and characters like &-/.")]
        public string Category { get; set; } = string.Empty;

        public IList<string> Image { get; set; } = new List<string>();

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 9999999.99, ErrorMessage = "Price must be between 0.01 and 9999999.99.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters.")]
        public string Description { get; set; } = string.Empty;

        public Dictionary<string, string> Specification { get; set; } = new();
    }
}