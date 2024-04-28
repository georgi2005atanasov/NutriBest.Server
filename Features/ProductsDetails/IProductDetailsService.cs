namespace NutriBest.Server.Features.ProductsDetails
{
    public interface IProductDetailsService
    {
        Task AddDetails(
            int productId,
            string? howToUse,
            string? servingSize,
            string? servingsPerContainer);

        Task RemoveDetails(int productId, string name);
    }
}
