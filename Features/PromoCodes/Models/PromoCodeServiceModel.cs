namespace NutriBest.Server.Features.PromoCodes.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.PromoCodes;

    public class PromoCodeServiceModel
    {
        public string DiscountPercentage { get; set; } = null!;

        [Required]
        public string Count { get; set; } = null!;

        [StringLength(MaxDescriptionLength, MinimumLength = MinDescriptionLength)]
        public string Description { get; set; } = null!;

        public bool IsValid { get; set; }
    }
}