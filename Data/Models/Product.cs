using System.ComponentModel.DataAnnotations.Schema;

namespace NutriBest.Server.Data.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public string Description { get; set; } = null!;

        public int ProductImageId { get; set; }

        [NotMapped]
        public ProductImage ProductImage { get; set; } = null!;
    }
}
