
namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Category
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public List<ProductCategory> ProductsCategories { get; set; } = new List<ProductCategory>();
    }
}
