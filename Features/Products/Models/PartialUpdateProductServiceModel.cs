namespace NutriBest.Server.Features.Products.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.Product;

    public class PartialUpdateProductServiceModel
    {
        [Required]
        [StringLength(MaxDescriptionLength, MinimumLength = 5)]
        public string? Description { get; set; }
    }
}
