namespace NutriBest.Server.Features.Reports.Models
{
    using NutriBest.Server.Features.Products.Models;

    public class SellingProductServiceModel
    {
        public int SoldCount { get; set; }

        public ProductListingServiceModel Product { get; set; } = new ProductListingServiceModel();
    }
}
