namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Address
    {
        public int Id { get; set; }

        [Required]
        public string? ProfileId { get; set; }

        public Profile? Profile { get; set; }

        [Required]
        public string Street { get; set; } = null!;

        public string? StreetNumber { get; set; }

        [Required]
        public int CountryId { get; set; }

        public Country Country { get; set; } = null!;

        [Required]
        public int CityId { get; set; }

        public City? City { get; set; }

        public int? PostalCode { get; set; }

        public bool IsAnonymous { get; set; }

        public bool IsDeleted { get; set; }
    }
}
