namespace NutriBest.Server.Features.Reports.Models
{
    public class PerformanceInfo
    {
        public decimal OverallSalesVolume { get; set; }

        public List<SellingProductServiceModel> TopSellingProducts { get; set; } = new List<SellingProductServiceModel>();

        public List<SellingBrandServiceModel> TopSellingBrands { get; set; } = new List<SellingBrandServiceModel>();

        public List<SellingCategoryServiceModel> TopSellingCategories { get; set; } = new List<SellingCategoryServiceModel>();

        public List<SellingFlavourServiceModel> TopSellingFlavours { get; set; } = new List<SellingFlavourServiceModel>();

        public List<SellingProductServiceModel> WeakProducts { get; set; } = new List<SellingProductServiceModel>();

        public List<SellingBrandServiceModel> WeakBrands { get; set; } = new List<SellingBrandServiceModel>();

        public List<SellingCategoryServiceModel> WeakCategories { get; set; } = new List<SellingCategoryServiceModel>();

        public List<SellingFlavourServiceModel> WeakFlavours { get; set; } = new List<SellingFlavourServiceModel>();
    }
}
