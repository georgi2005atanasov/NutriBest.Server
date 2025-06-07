namespace NutriBest.Server.Features.ProductsDetails
{
    using NutriBest.Server.Features.ProductsDetails.Models;

    public interface IProductsDetailsService
    {
        Task AddDetails(
            int productId,
            string? howToUse,
            string? servingSize,
            string? whyChoose,
            string? ingredients);

        Task RemoveDetails(int productId);

        Task<ProductDetailsServiceModel> GetById(int id);
    }
}
