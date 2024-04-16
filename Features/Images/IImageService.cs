namespace NutriBest.Server.Features.Images
{
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Images.Models;

    public interface IImageService
    {
        Task<ProductImage> CreateImage(IFormFile image, string contentType);

        Task<ImageListingModel> GetImageByProductId(int productId);
    }
}
