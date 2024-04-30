namespace NutriBest.Server.Features.ProductsPromotions.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ProductPromotionServiceModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int PromotionId { get; set; }
    }
}
