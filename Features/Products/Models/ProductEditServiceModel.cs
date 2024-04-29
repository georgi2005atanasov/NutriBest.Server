using NutriBest.Server.Data;

namespace NutriBest.Server.Features.Products.Models
{
    using NutriBest.Server.Features.Images.Models;
    using System.ComponentModel.DataAnnotations;
    using static Validation.Product;

    public class ProductEditServiceModel
    {
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [Range(MinPrice, MaxPrice)]
        public decimal Price { get; set; }

        public int? Quantity { get; set; }

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public List<string> Categories { get; set; } = new List<string>();

        [Required]
        public ImageListingServiceModel Image { get; set; } = null!;
    }
}
