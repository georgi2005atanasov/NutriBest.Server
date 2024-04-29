namespace NutriBest.Server.Features.Promotions
{
    using NutriBest.Server.Features.Promotions.Models;

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
