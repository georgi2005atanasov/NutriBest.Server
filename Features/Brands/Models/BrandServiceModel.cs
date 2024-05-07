namespace NutriBest.Server.Features.Brands.Models
{
    using NutriBest.Server.Features.Images.Models;

    public class BrandServiceModel
    {
        public string Name { get; set; } = null!;

        public int? BrandLogoId { get; set; }

        public ImageListingServiceModel? BrandLogo { get; set; }
    }
}
