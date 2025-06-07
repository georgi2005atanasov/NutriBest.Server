namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static Validation.Package;

    public class Package
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Range(MinSize, MaxSize)]
        public int Grams { get; set; }

        public bool IsDeleted { get; set; }

        public List<ProductPackageFlavour> ProductPackageFlavours { get; set; } = new List<ProductPackageFlavour>();
    }
}
