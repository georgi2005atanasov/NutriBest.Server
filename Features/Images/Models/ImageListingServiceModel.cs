namespace NutriBest.Server.Features.Images.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ImageListingServiceModel
    {
        [Required]
        public string ImageData { get; set; } = null!;

        [Required]
        public string ContentType { get; set; } = null!;
    }
}
