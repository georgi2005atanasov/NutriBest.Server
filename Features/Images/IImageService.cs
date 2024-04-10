namespace NutriBest.Server.Features.Images
{
    using NutriBest.Server.Data.Models;
    public interface IImageService
    {
        Task<ProductImage> GetImage(IFormFile image, string contentType);
    }
}
