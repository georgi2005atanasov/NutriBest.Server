namespace NutriBest.Server.Features.Products.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.Product;

    public class CreateProductRequestModel
    {
        [Required]
        [StringLength(MaxNameLength, MinimumLength = 1)]
        public string Name { get; set; } = null!;

        [Required]
        public string? Price { get; set; } = null;

        public int? Quantity { get; set; }

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public List<string> Categories { get; set; } = null!;

        [Required]
        public IFormFile Image { get; set; } = null!;
    }
}
