namespace NutriBest.Server.Features.ProductsDetails.Models
{
    using NutriBest.Server.Features.Images.Models;
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.Product;

    public class ProductDetailsServiceModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(MinPrice, MaxPrice)]
        public decimal Price { get; set; }

        public int? Quantity { get; set; }

        public int? PromotionId { get; set; }

        [Required]
        [StringLength(MaxNameLength, MinimumLength = 5)]
        public string Description { get; set; } = null!;

        public string Brand { get; set; } = null!;

        [Required]
        public List<string> Categories { get; set; } = new List<string>();

        [Required]
        public ImageListingServiceModel Image { get; set; } = null!;

        public string? HowToUse { get; set; }

        public string? ServingSize { get; set; }
    }
}
