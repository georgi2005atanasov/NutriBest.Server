namespace NutriBest.Server.Features.Products
{
    using NutriBest.Server.Features.Products.Models;

    public interface IProductService
    {
        Task<int> Create(string name,
            string description,
            string brand,
            List<int> categoriesIds,
            List<ProductSpecsServiceModel> productSpecs,
            string imageData,
            string contentType);

        Task<AllProductsServiceModel> All(int page,
            string? categories,
            string? brand,
            string? priceFilter,
            string? alphaFilter,
            string? productsView,
            string? search,
            string? priceRange,
            string? quantities,
            string? flavours);

        Task<ProductServiceModel> Get(int id);

        Task<List<ProductSpecsServiceModel>> GetSpecs(int id, string name);

        Task<List<ProductListingServiceModel>> GetRelatedProducts(List<string>? categories, int productId);

        Task<ProductWithPromotionDetailsServiceModel> GetWithPromotion(int productId, int promotionId);

        Task<ProductPriceQuantityServiceModel?> GetCurrentPriceWithQuantity(int productId, 
            string flavour, 
            int grams);

        Task<int> Update(int id,
            string name,
            string description,
            string brand,
            List<int> categoriesIds,
            List<ProductSpecsServiceModel> productSpecs,
            string imageData,
            string contentType);

        //Task<bool> BuldEdit(string category,
        //    string brand,
        //    bool hasPromotion,
        //    decimal priceChange,
        //    decimal quantityChange);

        Task<int> PartialUpdate(int id, string? description);

        Task<bool> Delete(int productId);
    }
}
