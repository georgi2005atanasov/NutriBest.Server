namespace NutriBest.Server.Features.Promotions.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.Promotion;

    public class CreatePromotionServiceModel
    {
        [StringLength(MaxDescriptionLength, MinimumLength = MinDescriptionLength)]
        public string? Description { get; set; }

        public string? DiscountPercentage { get; set; }

        public string? DiscountAmount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; }

        public string? Category { get; set; }

        public string? Brand { get; set; }
    }
}
