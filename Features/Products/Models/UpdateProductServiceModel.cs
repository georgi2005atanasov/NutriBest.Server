namespace NutriBest.Server.Features.Products.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.Brand;

    public class UpdateProductServiceModel
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        [StringLength(MaxBrandLength)]
        public string Brand { get; set; } = null!;

        [Required]
        public List<string> Categories { get; set; } = null!;

        [Required]
        public string ProductSpecs { get; set; } = null!;

        public IFormFile? Image { get; set; }
    }
}
