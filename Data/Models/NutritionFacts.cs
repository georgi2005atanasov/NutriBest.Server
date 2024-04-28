namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class NutritionFacts
    {
        [Key]
        [Required]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        public double? EnergyValue { get; set; }

        public double? Fats { get; set; }

        public double? SaturatedFats { get; set; }

        public double? Carbohydrates { get; set; }

        public double? Sugars { get; set; }

        public double? Proteins { get; set; }

        public double? Salt { get; set; }

        public bool IsDeleted { get; set; }
    }
}