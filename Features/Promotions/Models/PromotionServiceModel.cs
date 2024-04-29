namespace NutriBest.Server.Features.Promotions.Models
{
    public class PromotionServiceModel
    {
        public string? Description { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public decimal? DiscountAmount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public decimal? SpecialPrice { get; set; }
    }
}
