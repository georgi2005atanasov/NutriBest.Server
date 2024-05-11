namespace NutriBest.Server.Features.Products.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.Package;

    public class ProductSpecsServiceModel
    {
        [Required]
        public string Flavour { get; set; } = null!;

        [Required]
        [Range(MinSize, MaxSize)]
        public int Grams { get; set; }

        [Required]
        [Range(MinQuantity, MaxQuantity)]
        public int Quantity{ get; set; }
    }
}
