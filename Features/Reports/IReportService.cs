namespace NutriBest.Server.Features.Reports
{
    using NutriBest.Server.Features.Reports.Models;

    public interface IReportService
    {
        Task<TopSellingProductsServiceModel> GetTopSellingProducts();

        Task<TopSellingBrandsServiceModel> GetTopSellingBrands();

        Task<TopSellingFlavoursServiceModel> GetTopSellingFlavours();

        Task<TopSellingCategoriesServiceModel> GetTopSellingCategories();
    }
}
