namespace NutriBest.Server.Features.Reports
{
    using NutriBest.Server.Features.Reports.Models;

    public interface IReportService
    {
        Task<PerformanceInfo> GetPerformanceInfo();

        Task<List<SellingProductServiceModel>> GetTopSellingProducts();

        Task<List<SellingBrandServiceModel>> GetTopSellingBrands();

        Task<List<SellingFlavourServiceModel>> GetTopSellingFlavours();

        Task<List<SellingCategoryServiceModel>> GetTopSellingCategories();

        Task<List<SellingProductServiceModel>> GetWeakProducts();

        Task<List<SellingBrandServiceModel>> GetWeakBrands();

        Task<List<SellingFlavourServiceModel>> GetWeakFlavours();

        Task<List<SellingCategoryServiceModel>> GetWeakCategories();

        Task<decimal?> GetOverallSalesVolume();

        Task<List<SellingCityServiceModel>> GetTopCities();
    }
}
