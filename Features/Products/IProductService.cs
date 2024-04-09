using NutriBest.Server.Data.Models;

namespace NutriBest.Server.Features.Products
{
    public interface IProductService
    {
        Task<ProductImage> GetImage(IFormFile image, string contentType);
        Task<int> Create(string name, 
            string description, 
            decimal price, 
            byte[] imageData,
            string contentType);
    }
}
