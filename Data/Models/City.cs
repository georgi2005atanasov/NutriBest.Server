namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static Validation.City;

    public class City
    {
        public int Id { get; set; }

        [MaxLength(CityNameMaxLength)]
        public string CityName { get; set; } = null!;

        public int? PostalCode { get; set; }

        public HashSet<Address> Addresses { get; set; } = new HashSet<Address>();
    }
}
