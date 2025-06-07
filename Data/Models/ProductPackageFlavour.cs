namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static Validation.Product;

    public class ProductPackageFlavour
    {
        [Required]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        [Required]
        public int PackageId { get; set; }

        public Package? Package { get; set; }

        [Required]
        public int FlavourId { get; set; }

        public Flavour? Flavour { get; set; }

        [Required]
        [Range(Validation.Package.MinQuantity, Validation.Package.MaxQuantity)]
        public int Quantity { get; set; }

        [Required]
        [Range(MinPrice, MaxPrice)]
        public decimal Price { get; set; }

        public bool IsDeleted { get; set; }
    }
}
