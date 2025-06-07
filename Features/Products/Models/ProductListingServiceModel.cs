namespace NutriBest.Server.Features.Products.Models
{
    public class ProductListingServiceModel : ProductServiceModel
    {
        public int? PromotionId { get; set; }

        public decimal? DiscountPercentage { get; set; }
    }
}