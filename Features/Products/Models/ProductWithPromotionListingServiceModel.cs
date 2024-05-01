namespace NutriBest.Server.Features.Products.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.Product;

    public class ProductWithPromotionDetailsServiceModel : ProductServiceModel
    {
        [Required]
        [Range(MinPrice, MaxPrice)]
        public decimal NewPrice { get; set; }

        public decimal? DiscountAmount { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public int? PromotionId { get; set; }
    }
}
