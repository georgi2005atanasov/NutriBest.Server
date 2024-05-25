namespace NutriBest.Server.Features.PromoCodes.Models
{
    using System.ComponentModel.DataAnnotations;

    public class PromoCodeServiceModel
    {
        public decimal DiscountPercentage { get; set; }

        [Required]
        public int Count { get; set; }

        public string Description { get; set; } = null!;

        public bool IsValid { get; set; }
    }
}