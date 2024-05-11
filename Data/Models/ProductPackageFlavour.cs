namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static Validation.Package;

    public class ProductPackageFlavour
    {
        [Required]
        public int ProductId { get; set; }

        [NotMapped]
        public Product? Product { get; set; }

        [Required]
        public int PackageId { get; set; }

        public Package? Package { get; set; }

        [Required]
        public int FlavourId { get; set; }

        public Flavour? Flavour { get; set; }

        [Required]
        [Range(MinQuantity, MaxQuantity)]
        public int Quantity { get; set; }

        public bool IsDeleted { get; set; }
    }
}
