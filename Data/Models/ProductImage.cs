using System.ComponentModel.DataAnnotations.Schema;

namespace NutriBest.Server.Data.Models
{
    public class ProductImage
    {
        public int ProductImageId { get; set; }
        public byte[] ImageData { get; set; } = null!;
        public string ContentType { get; set; } = null!;

        [NotMapped]
        public int ProductId { get; set; }

        [NotMapped]
        public Product Product { get; set; } = null!;
    }
}
