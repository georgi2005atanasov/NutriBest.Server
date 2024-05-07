namespace NutriBest.Server.Features.Categories.Models
{
    using System.ComponentModel.DataAnnotations;

    public class CreateCategoryServiceModel
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
