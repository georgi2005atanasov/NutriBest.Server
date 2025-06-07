namespace NutriBest.Server.Features.ShippingDiscounts.Models
{
    public class ShippingDiscountServiceModel
    {
        public string CountryName { get; set; } = null!;

        public decimal DiscountPercentage { get; set; }

        public decimal? MinimumPrice { get; set; }

        public string Description { get; set; } = null!;

        public DateTime? EndDate { get; set; }
    }
}
