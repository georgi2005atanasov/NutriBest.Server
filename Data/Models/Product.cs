namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static WebConstants.ProductConstants;

    public class Product
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(MinPrice, MaxPrice)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(MaxNameLength, MinimumLength = 5)]
        public string Description { get; set; } = null!;

        [Required]
        public DateTime CreatedOn { get; set; }

        public int ProductImageId { get; set; }

        [NotMapped]
        public ProductImage ProductImage { get; set; } = null!;

        [Required]
        public List<ProductCategory> ProductsCategories { get; set; } = new List<ProductCategory>();
    }
}
