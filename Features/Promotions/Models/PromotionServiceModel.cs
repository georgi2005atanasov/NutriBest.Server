namespace NutriBest.Server.Features.Promotions.Models
{
    public class PromotionServiceModel
    {
        public int? PromotionId { get; set; }

        public string? Description { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public decimal? DiscountAmount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; }

        public string? Category { get; set; }

        public string? Brand { get; set; }
    }
}
