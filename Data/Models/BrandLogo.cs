namespace NutriBest.Server.Data.Models
{
    using NutriBest.Server.Features.Images.Models;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class BrandLogo : IFileData
    {
        [Key]
        [Required]
        public int BrandLogoId { get; set; }

        [NotMapped]
        public Brand? Brand { get; set; }

        [Required]
        public string ImageData { get; set; } = null!;

        [Required]
        public string ContentType { get; set; } = null!;
    }
}
