namespace NutriBest.Server.Features.Promotions.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.Promotion;

    public class CreatePromotionServiceModel
    {
        [StringLength(MaxDescriptionLength, MinimumLength = MinDescriptionLength)]
        public string? Description { get; set; }

        [Range(MinPercentage, MaxPercentage)]
        public string? DiscountPercentage { get; set; }

        [Range(MinPrice, MaxPrice)]
        public string? DiscountAmount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; }

        public string? MinimumPrice { get; set; }

        public string? Category { get; set; }
    }
}
