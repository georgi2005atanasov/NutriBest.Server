namespace NutriBest.Server.Features.PromoCodes.Models
{
    public class PromoCodeListingModel
    {
        public string Code { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal DiscountPercentage { get; set; }

        public bool IsValid { get; set; }
    }
}
