namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ProductCategory
    {
        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        [Required]
        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }
}
