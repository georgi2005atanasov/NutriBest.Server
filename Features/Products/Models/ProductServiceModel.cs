namespace NutriBest.Server.Features.Products.Models
{
    using NutriBest.Server.Features.Images.Models;
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.Product;

    public class ProductServiceModel
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(MaxNameLength, MinimumLength = MinNameLength)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(MinPrice, MaxPrice)]
        public decimal Price { get; set; }

        [Range(MinQuantity, MaxQuantity)]
        public int? Quantity { get; set; }

        [Required]
        [StringLength(MaxDescriptionLength, MinimumLength = MinDescriptionLength)]
        public string Description { get; set; } = null!;

        public string Brand { get; set; } = null!;

        [Required]
        public List<string> Categories { get; set; } = new List<string>();

        public ImageListingServiceModel Image { get; set; } = null!;
    }
}
