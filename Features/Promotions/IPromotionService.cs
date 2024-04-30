namespace NutriBest.Server.Features.Promotions
{
    using NutriBest.Server.Features.Promotions.Models;

    public interface IPromotionService
    {
        Task<PromotionServiceModel> Get(int promotionId);

        Task<bool> CreateProductPromotion(int productId,
            int promotionId,
            decimal? specialPrice); // new shit

        Task<int> Create(string? description,
            decimal? discountAmount,
            decimal? discountPercentage,
            DateTime startDate,
            DateTime endDate,
            decimal? specialPrice);

        Task<bool> Update(int promotionId,
            string? description,
            decimal? discountAmount,
            decimal? discountPercentage,
            decimal? specialPrice);

        Task<bool> Remove(int promotionId);
    }
}
