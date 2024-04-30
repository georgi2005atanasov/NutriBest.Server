namespace NutriBest.Server.Features.Promotions.Models
{
    public class UpdatePromotionServiceModel
    {
        public string? Description { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public decimal? DiscountAmount { get; set; }

        public decimal? SpecialPrice { get; set; }
    }
}
