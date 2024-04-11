namespace NutriBest.Server.Features.Images
{
    using NutriBest.Server.Data.Models;
    public interface IImageService
    {
        Task<ProductImage> CreateImage(IFormFile image, string contentType);
    }
}
