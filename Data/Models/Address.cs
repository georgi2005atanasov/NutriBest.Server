namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

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
        public int CountryId { get; set; }

        public Country Country { get; set; } = null!;

        [Required]
        public int CityId { get; set; }

        public City? City { get; set; }

        public bool IsAnonymous { get; set; }
    }
}
