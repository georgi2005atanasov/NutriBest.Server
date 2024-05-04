namespace NutriBest.Server.Features.Promotions.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.Promotion;

    public class UpdatePromotionServiceModel
    {
        [StringLength(MaxDescriptionLength, MinimumLength = MinDescriptionLength)]
        public string? Description { get; set; }

        [Range(MinPercentage, MaxPercentage)]
        public string? DiscountPercentage { get; set; }

        [Range(MinPrice, MaxPrice)]
        public string? DiscountAmount { get; set; }

        public string? MinimumPrice { get; set; }

        public string? Category { get; set; }
    }
}
