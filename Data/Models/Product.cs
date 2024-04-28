﻿namespace NutriBest.Server.Data.Models
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
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(MinPrice, MaxPrice)]
        public decimal Price { get; set; }

        public int? Quantity { get; set; }

        [Required]
        [StringLength(DescriptionMaxLength, MinimumLength = 5)]
        public string Description { get; set; } = null!;

        public int ProductImageId { get; set; }

        [NotMapped]
        public ProductImage ProductImage { get; set; } = null!;

        public ProductDetails? ProductDetails { get; set; }

        public NutritionFacts? NutritionFacts { get; set; }

        public List<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

        [Required]
        public List<ProductCategory> ProductsCategories { get; set; } = new List<ProductCategory>();

        public List<ProductPromotion> ProductPromotions { get; set; } = new List<ProductPromotion>();
    }
}
