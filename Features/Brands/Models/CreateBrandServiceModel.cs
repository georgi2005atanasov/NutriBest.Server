namespace NutriBest.Server.Features.Brands.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.Brand;

    public class CreateBrandServiceModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(MaxBrandLength)]
        public string Name { get; set; } = null!;

        [StringLength(MaxBrandDescriptionLength)]
        public string? Description { get; set; }

        public IFormFile? Image { get; set; }
    }
}
