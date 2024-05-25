namespace NutriBest.Server.Features.PromoCodes.Models
{
    using System.ComponentModel.DataAnnotations;

    public class PromoCodeServiceModel
    {
        public decimal DiscountPercentage { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public string Description { get; set; } = null!;
    }
}