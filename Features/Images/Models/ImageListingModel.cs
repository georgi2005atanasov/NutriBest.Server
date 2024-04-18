using System.ComponentModel.DataAnnotations;

namespace NutriBest.Server.Features.Images.Models
{
    public class ImageListingModel
    {
        [Required]
        public string ImageData { get; set; } = null!;

        [Required]
        public string ContentType { get; set; } = null!;
    }
}
