namespace NutriBest.Server.Features.Reports
{
    using NutriBest.Server.Features.Reports.Models;

    public interface IReportService
    {
        Task<PerformanceInfo> GetPerformanceInfo(DateTime? startDate, DateTime? endDate);

        Task<List<SellingProductServiceModel>> GetTopSellingProducts(DateTime? startDate, DateTime? endDate);

        Task<List<SellingBrandServiceModel>> GetTopSellingBrands(DateTime? startDate, DateTime? endDate);

        Task<List<SellingFlavourServiceModel>> GetTopSellingFlavours(DateTime? startDate, DateTime? endDate);

        Task<List<SellingCategoryServiceModel>> GetTopSellingCategories(DateTime? startDate, DateTime? endDate);

        Task<List<SellingProductServiceModel>> GetWeakProducts(DateTime? startDate, DateTime? endDate);

        Task<List<SellingBrandServiceModel>> GetWeakBrands(DateTime? startDate, DateTime? endDate);

        Task<List<SellingFlavourServiceModel>> GetWeakFlavours(DateTime? startDate, DateTime? endDate);

        Task<List<SellingCategoryServiceModel>> GetWeakCategories(DateTime? startDate, DateTime? endDate);

        Task<decimal?> GetOverallSalesVolume(DateTime? startDate, DateTime? endDate);

        Task<List<SellingCityServiceModel>> GetTopCities(DateTime? startDate, DateTime? endDate);
    }
}
