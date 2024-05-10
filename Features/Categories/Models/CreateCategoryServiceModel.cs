namespace NutriBest.Server.Features.Categories.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.Category;

    public class CreateCategoryServiceModel
    {
        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; } = null!;
    }
}
