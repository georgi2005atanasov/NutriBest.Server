namespace NutriBest.Server.Features.ProductsDetails
{
    using NutriBest.Server.Features.ProductsDetails.Models;

    public interface IProductDetailsService
    {
        Task AddDetails(
            int productId,
            string? howToUse,
            string? servingSize);

        Task RemoveDetails(int productId);

        Task<ProductDetailsServiceModel> GetById(int id);
    }
}
