namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static Validation.Category;

    public class Category
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; } = null!;

        public List<ProductCategory> ProductsCategories { get; set; } = new List<ProductCategory>();
    }
}
