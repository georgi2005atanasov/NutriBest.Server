namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static Validation.Country;
    public class Country
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(CountryNameMaxLength)]
        public string CountryName { get; set; } = null!;

        public string? IsoCode { get; set; }

        public decimal ShippingPrice { get; set; }

        public int? ShippingDiscountId { get; set; }

        public ShippingDiscount? ShippingDiscount { get; set; }

        public HashSet<Address> Addresses { get; set; } = new HashSet<Address>();

        public HashSet<City> Cities { get; set; } = new HashSet<City>();
    }
}
