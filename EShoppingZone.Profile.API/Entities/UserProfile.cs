using System.ComponentModel.DataAnnotations;

namespace EShoppingZone.Profile.API.Entities
{
    public class UserProfile
    {
        [Key]
        public int ProfileId { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string EmailId { get; set; } = string.Empty;

        [Required]
        public long MobileNumber { get; set; }

        [MaxLength(300)]
        public string? About { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(20)]
        public string? Gender { get; set; }

        [Required, MaxLength(30)]
        public string Role { get; set; } = "CUSTOMER";

        [Required]
        public string Password { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Image { get; set; }

        public IList<Address> Addresses { get; set; } = new List<Address>();
    }
}