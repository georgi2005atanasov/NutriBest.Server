namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Address
    {
        public int Id { get; set; }

        [Required]
        public string ProfileId { get; set; } = null!;

        public Profile? Profile { get; set; }

        [Required]
        public string Street { get; set; } = null!;

        public int? StreetNumber { get; set; }

        [Required]
        public string Country { get; set; } = null!;

        [Required]
        public string City { get; set; } = null!;

        public int? PostalCode { get; set; }
    }
}
