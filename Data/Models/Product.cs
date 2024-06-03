namespace NutriBest.Server.Data.Models
{
    using NutriBest.Server.Data.Models.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static Validation.Product;

    public class Product : DeletableEntity
    {
        [Required]
        public int ProductId { get; set; }

        [Required] 
        [StringLength(MaxNameLength, MinimumLength = MinNameLength)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(MinPrice, MaxPrice)]
        public decimal StartingPrice { get; set; }

        [Required]
        [Range(MinPrice, MaxPrice)]
        public decimal MaximumPrice { get; set; }

        [Range(MinQuantity, MaxQuantity)]
        public int? Quantity { get; set; }

        [Required]
        [StringLength(MaxDescriptionLength, MinimumLength = MinDescriptionLength)]
        public string Description { get; set; } = null!;

        public int? BrandId { get; set; }

        public Brand? Brand { get; set; }

        public int ProductImageId { get; set; }

        [NotMapped]
        public ProductImage? ProductImage { get; set; }

        public int? PromotionId { get; set; }

        public Promotion? Promotion { get; set; }

        public ProductDetails ProductDetails { get; set; } = new ProductDetails();

        public NutritionFacts NutritionFacts { get; set; } = new NutritionFacts();

        [Required]
        public List<ProductCategory> ProductsCategories { get; set; } = new List<ProductCategory>();

        public List<ProductPackageFlavour> ProductPackageFlavours { get; set; } = new List<ProductPackageFlavour>();
    }
}
