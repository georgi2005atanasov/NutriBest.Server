namespace NutriBest.Server.Features.Products
{
    using NutriBest.Server.Features.Products.Models;

    public interface IProductService
    {
        Task<int> Create(string name,
            string description,
            double price,
            List<int> categoriesIds,
            string imageData,
            string contentType);

        Task<AllProductsModel> All(int page,
            string? categories,
            string? priceFilter);

        Task<ProductDetailsModel?> GetById(int id);

        Task<int> Update(int id,
            string name,
            string description,
            double price,
            List<int> categoriesIds,
            string imageData,
            string contentType);

        Task<bool> Delete(int productId);
    }
}
