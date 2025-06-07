namespace NutriBest.Server.Features.Export
{
    using NutriBest.Server.Features.Brands.Models;
    using NutriBest.Server.Features.Orders.Models;
    using NutriBest.Server.Features.Reports.Models;
    using NutriBest.Server.Features.Profile.Models;
    using NutriBest.Server.Features.Products.Models;
    using NutriBest.Server.Features.Packages.Models;
    using NutriBest.Server.Features.Flavours.Models;
    using NutriBest.Server.Features.Newsletter.Models;
    using NutriBest.Server.Features.Categories.Models;
    using NutriBest.Server.Features.PromoCodes.Models;
    using NutriBest.Server.Features.Promotions.Models;
    using NutriBest.Server.Features.ShippingDiscounts.Models;

    public interface IExportService
    {
        string BrandsCsv(IEnumerable<BrandServiceModel> brands);

        string CategoriesCsv(IEnumerable<CategoryServiceModel> categories);

        string FlavoursCsv(IEnumerable<FlavourServiceModel> flavours);

        string NewsletterCsv(IEnumerable<SubscriberServiceModel> subscribers);

        string OrdersCsv(IEnumerable<OrderListingServiceModel> orders);

        string OrdersSummaryCsv(OrdersSummaryServiceModel summary);

        string PackagesCsv(IEnumerable<PackageServiceModel> packages);

        string ProductsCsv(IEnumerable<ProductListingServiceModel> products);

        string ProfilesCsv(IEnumerable<ProfileListingServiceModel> profiles);

        string PromoCodesCsv(IEnumerable<PromoCodeByDescriptionServiceModel> promoCodes);

        string PromotionsCsv(IEnumerable<PromotionServiceModel> promotions);

        string PerformanceInfoCsv(PerformanceInfo report);

        string DemographicsInfoCsv(IEnumerable<SellingCityServiceModel> data);

        string ShippingDiscountsCsv(IEnumerable<ShippingDiscountServiceModel> data);
    }
}
