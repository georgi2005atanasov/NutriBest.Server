
namespace NutriBest.Server.Features.Products.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ProductListingModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;
    }
}
