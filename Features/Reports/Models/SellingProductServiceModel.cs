namespace NutriBest.Server.Features.Reports.Models
{
    using NutriBest.Server.Features.Products.Models;

    public class SellingProductServiceModel
    {
        public ProductListingServiceModel Product { get; set; } = new ProductListingServiceModel();

        public int SoldCount { get; set; }
    }
}
