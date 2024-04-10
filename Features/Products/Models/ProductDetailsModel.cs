namespace NutriBest.Server.Features.Products.Models
{
    using System.ComponentModel.DataAnnotations;
    using static WebConstants.ProductConstants;

    public class ProductDetailsModel
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
    }
}
