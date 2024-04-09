using System.ComponentModel.DataAnnotations;

namespace NutriBest.Server.Features.Products
{
    public class CreateProductRequestModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public IFormFile Image { get; set; }
    }
}
