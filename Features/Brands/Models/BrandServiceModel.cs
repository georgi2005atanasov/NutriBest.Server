namespace NutriBest.Server.Features.Brands.Models
{
    using NutriBest.Server.Features.Images.Models;

    public class BrandServiceModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int? BrandLogoId { get; set; }

        public ImageListingServiceModel? BrandLogo { get; set; }
    }
}
