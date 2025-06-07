namespace NutriBest.Server.Features.Promotions.Models
{
    public class PromotionValueServiceModel
    {
        public int? PromotionId { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public decimal? DiscountAmount { get; set; }
    }
}
