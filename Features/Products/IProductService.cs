namespace NutriBest.Server.Features.Products
{
    using NutriBest.Server.Features.Products.Models;

    public interface IProductService
    {
        Task<int> Create(string name, 
            string description, 
            decimal price,
            List<int> categoriesIds,
            byte[] imageData,
            string contentType);

        Task<IEnumerable<ProductListingModel>> All();

        Task<ProductDetailsModel?> GetById(int id);

        Task<int> Update(int id,
            string name,
            string description,
            decimal price,
            List<int> categoriesIds,
            byte[] imageData,
            string contentType);
    }
}
