using System.ComponentModel.DataAnnotations;

namespace NutriBest.Server.Features.Products.Models
{
    public class ProductListingModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;
    }
}
