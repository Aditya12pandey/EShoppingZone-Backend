using System.ComponentModel.DataAnnotations;

namespace EShoppingZone.Order.API.Entities
{
    public class DeliveryAddress
    {
        [Required(ErrorMessage = "Full name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name can only contain letters and spaces.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be exactly 10 digits.")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Flat number is required.")]
        public string FlatNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City can only contain letters and spaces.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "State can only contain letters and spaces.")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pincode is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Pincode must be exactly 6 digits.")]
        public string Pincode { get; set; } = string.Empty;
    }
}