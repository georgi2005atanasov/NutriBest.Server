namespace NutriBest.Server.Features.Products.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ProductSpecsServiceModel
    {
        [Required]
        public string Flavour { get; set; } = null!;

        [Required]
        public int Grams { get; set; }

        [Required]
        public int Quantity{ get; set; }
    }
}
