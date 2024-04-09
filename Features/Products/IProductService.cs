using NutriBest.Server.Data.Models;
using NutriBest.Server.Features.Products.Models;

namespace NutriBest.Server.Features.Products
{
    public interface IProductService
    {
        Task<ProductImage> GetImage(IFormFile image, string contentType);
        Task<int> Create(string name, 
            string description, 
            decimal price, 
            List<string> categories,
            byte[] imageData,
            string contentType);

        Task<IEnumerable<ProductListingModel>> All();
    }
}
