using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShoppingZone.Profile.API.Entities
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [Required, MaxLength(20)]
        public string HouseNumber { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string StreetName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? ColonyName { get; set; }

        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string Pincode { get; set; } = string.Empty;

        [ForeignKey("UserProfile")]
        public int ProfileId { get; set; }
        public UserProfile UserProfile { get; set; } = null!;
    }
}