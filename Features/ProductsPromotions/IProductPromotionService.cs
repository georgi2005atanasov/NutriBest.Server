namespace NutriBest.Server.Features.ProductsPromotions
{
    public interface IProductPromotionService
    {
        Task<bool> Create(int productId,
            int promotionId);

        Task<bool> Remove(int productId);
    }
}
