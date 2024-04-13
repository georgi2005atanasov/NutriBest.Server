
namespace NutriBest.Server.Features.Products.Models
{
    using NutriBest.Server.Data.Models;
    using System.ComponentModel.DataAnnotations;

    public class ProductListingModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public ImageListingModel ProductImage { get; set; } = null!;
    }
}
