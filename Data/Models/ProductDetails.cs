using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NutriBest.Server.Data.Models
{
    public class ProductDetails
    {
        [Key]
        [Required]
        public int ProductId { get; set; }

        [NotMapped]
        public Product? Product { get; set; }

        public string? HowToUse { get; set; }

        public string? ServingSize { get; set; }

        public string? NutritionFacts { get; set; }

        public bool IsDeleted { get; set; }
    }
}
