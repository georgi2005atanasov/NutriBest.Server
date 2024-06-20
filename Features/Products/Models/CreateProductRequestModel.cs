namespace NutriBest.Server.Features.Products.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.Product;
    using static ServicesConstants.Brand;

    public class CreateProductRequestModel
    {
        [Required]
        [StringLength(MaxNameLength, MinimumLength = 1)]
        public string Name { get; set; } = null!;

        [Required]
        public string? Price { get; set; } = null;

        [Required]
        [StringLength(MaxDescriptionLength, MinimumLength = 5)]
        public string Description { get; set; } = null!;

        [Required]
        [StringLength(MaxBrandLength)]
        public string Brand { get; set; } = null!;

        [Required]
        public List<string> Categories { get; set; } = null!;

        public string ProductSpecs { get; set; } = null!;

        public IFormFile Image { get; set; } = null!;
    }
}
