using System.ComponentModel.DataAnnotations;

namespace NutriBest.Server.Features.Products.Models
{
    public class ImageListingModel
    {
        [Required]
        public string ImageData { get; set; } = null!;

        [Required]
        public string ContentType { get; set; } = null!;
    }
}
