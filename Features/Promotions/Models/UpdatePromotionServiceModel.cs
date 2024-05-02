namespace NutriBest.Server.Features.Promotions.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.Promotion;

    public class UpdatePromotionServiceModel
    {
        [StringLength(MaxDescriptionLength, MinimumLength = MinDescriptionLength)]
        public string? Description { get; set; }

        [Range(MinPercentage, MaxPercentage)]
        public decimal? DiscountPercentage { get; set; }

        [Range(MinPrice, MaxPrice)]
        public decimal? DiscountAmount { get; set; }

        public decimal? MinimumPrice { get; set; }

        public string? Category { get; set; }
    }
}
