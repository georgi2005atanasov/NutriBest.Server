namespace NutriBest.Server.Features.Products.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UpdateProductModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public List<string> Categories { get; set; } = null!;

        [Required]
        public IFormFile Image { get; set; } = null!;
    }
}
