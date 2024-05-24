using System.ComponentModel.DataAnnotations;

namespace NutriBest.Server.Features.PromoCodes.Models
{
    public class PromoCodeServiceModel
    {
        public decimal? DiscountPercentage { get; set; }

        public decimal? DiscountAmount { get; set; }

        [Required]
        public int Count { get; set; }
    }
}