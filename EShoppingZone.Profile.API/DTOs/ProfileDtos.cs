using System.ComponentModel.DataAnnotations;

namespace EShoppingZone.Profile.API.DTOs
{
    public class RegisterCustomerDto
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name can only contain letters and spaces.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
        public string EmailId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be exactly 10 digits.")]
        public long MobileNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must have 8+ chars, 1 uppercase, 1 number, 1 special character.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required.")]
        [RegularExpression(@"^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female or Other.")]
        public string Gender { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "About cannot exceed 500 characters.")]
        public string About { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; }
    }

    public class RegisterMerchantDto
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name can only contain letters and spaces.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
        public string EmailId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be exactly 10 digits.")]
        public long MobileNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must have 8+ chars, 1 uppercase, 1 number, 1 special character.")]
        public string Password { get; set; } = string.Empty;

        [RegularExpression(@"^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female or Other.")]
        public string Gender { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "About cannot exceed 500 characters.")]
        public string About { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }
    }

    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
        public string EmailId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int ProfileId { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class ProfileUpdateDto
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name can only contain letters and spaces.")]
        public string? FullName { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be exactly 10 digits.")]
        public long? MobileNumber { get; set; }

        [StringLength(500, ErrorMessage = "About cannot exceed 500 characters.")]
        public string? About { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [RegularExpression(@"^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female or Other.")]
        public string? Gender { get; set; }

        public string? Image { get; set; }
    }

    public class AddressDto
    {
        [Required(ErrorMessage = "Profile ID is required.")]
        public int ProfileId { get; set; }

        [Required(ErrorMessage = "House number is required.")]
        [StringLength(20, ErrorMessage = "House number cannot exceed 20 characters.")]
        public string HouseNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Street name is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Street name must be between 3 and 200 characters.")]
        public string StreetName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Colony name is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Colony name must be between 3 and 200 characters.")]
        public string ColonyName { get; set; } = string.Empty;

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