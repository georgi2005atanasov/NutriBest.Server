namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    public class Flavour
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string FlavourName { get; set; } = null!;

        public bool IsDeleted { get; set; }

        public List<ProductPackageFlavour> ProductPackageFlavours { get; set; } = new List<ProductPackageFlavour>();
    }
}
