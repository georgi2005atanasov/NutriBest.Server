namespace NutriBest.Server.Features.ProductsPromotions
{
    using NutriBest.Server.Features.ProductsPromotions.Models;

    public interface IProductPromotionService
    {
        Task<PromotionServiceModel> Get(int promotionId);

        Task<int> Create(int productId, 
            string? description,
            decimal? discountAmount,
            decimal? discountPercentage,
            DateTime startDate,
            DateTime endDate,
            decimal? specialPrice);

        Task<bool> Remove(int promotionId);
    }
}
