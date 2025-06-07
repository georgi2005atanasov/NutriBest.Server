namespace NutriBest.Server.Data.Models
{
    using NutriBest.Server.Data.Models.Base;
    using NutriBest.Server.Features.Images.Models;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ProductImage : DeletableEntity, IFileData
    {
        public int ProductImageId { get; set; }

        [Required]
        public string ImageData { get; set; } = null!;

        [Required]
        public string ContentType { get; set; } = null!;

        [NotMapped]
        public int ProductId { get; set; }

        [NotMapped]
        public Product Product { get; set; } = null!;
    }
}
