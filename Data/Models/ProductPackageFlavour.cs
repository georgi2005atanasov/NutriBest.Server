namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

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

        public int Quantity { get; set; }

        public bool IsDeleted { get; set; }
    }
}
