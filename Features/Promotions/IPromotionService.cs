namespace NutriBest.Server.Features.Promotions
{
    using NutriBest.Server.Features.Promotions.Models;

    public interface IPromotionService
    {
        Task<IEnumerable<PromotionServiceModel>> All();

        Task<PromotionServiceModel> Get(int promotionId);

        //Task<bool> CreateCategoryPromotion(string category,
        //    int promotionId,
        //    decimal? specialPrice);

        Task<int> Create(string? description,
            decimal? discountAmount,
            decimal? discountPercentage,
            DateTime startDate,
            DateTime endDate);

        Task<bool> Update(int promotionId,
            string? description,
            decimal? discountAmount,
            decimal? discountPercentage);

        Task<bool> Remove(int promotionId);
    }
}
