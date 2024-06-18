namespace NutriBest.Server.Features.Reports.Models
{
    using NutriBest.Server.Features.Products.Models;

    public class TopSellingProductServiceModel
    {
        public int SoldCount { get; set; }

        public ProductListingServiceModel Product { get; set; } = new ProductListingServiceModel();
    }
}
