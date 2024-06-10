namespace NutriBest.Server.Features.Products.Models
{
    using NutriBest.Server.Features.Images.Models;

    public class OrderRelatedProductServiceModel
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = null!;

        public int? Quantity { get; set; }

        public ImageListingServiceModel Image { get; set; } = null!;

        public string Flavour { get; set; } = null!;

        public int Grams { get; set; }

        public decimal Price { get; set; }

        public int? PromotionId { get; set; }

        public decimal DiscountPercentage { get; set; }
    }
}
