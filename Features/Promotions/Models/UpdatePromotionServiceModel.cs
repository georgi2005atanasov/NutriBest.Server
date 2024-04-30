namespace NutriBest.Server.Features.Promotions.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.Promotion;

    public class UpdatePromotionServiceModel
    {
        public string? Description { get; set; }

        [Range(MinPercentage, MaxPercentage)]
        public decimal? DiscountPercentage { get; set; }

        [Range(MinPrice, MaxPrice)]
        public decimal? DiscountAmount { get; set; }
    }
}
