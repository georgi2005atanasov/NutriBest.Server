namespace NutriBest.Server.Data.Models
{
    using NutriBest.Server.Data.Models.Base;
    using System.ComponentModel.DataAnnotations;
    using static Validation.Promotion;

    public class ShippingDiscount : DeletableEntity
    {
        public int Id { get; set; }

        public int CountryId { get; set; }

        public Country? Country { get; set; }

        [Range(MinPercentage, MaxPercentage)]
        public decimal DiscountPercentage { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
