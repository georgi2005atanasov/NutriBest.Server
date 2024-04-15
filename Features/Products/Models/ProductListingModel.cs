
namespace NutriBest.Server.Features.Products.Models
{
    using NutriBest.Server.Data.Models;
    using System.ComponentModel.DataAnnotations;
    using static WebConstants.ProductConstants;

    public class ProductListingModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [Range(MinPrice, MaxPrice)]
        public double Price { get; set; }

        [Required]
        public ImageListingModel ProductImage { get; set; } = null!;

        public List<string> Categories { get; set; } = null!;
    }
}
