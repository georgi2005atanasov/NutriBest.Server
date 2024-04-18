namespace NutriBest.Server.Features.Products.Models
{
    using System.ComponentModel.DataAnnotations;
    using static WebConstants.ProductConstants;

    public class UpdateProductModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [Range(MinPrice, MaxPrice)]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public List<string> Categories { get; set; } = null!;

        public IFormFile? Image { get; set; }

        //more...
    }
}
