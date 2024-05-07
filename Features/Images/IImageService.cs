namespace NutriBest.Server.Features.Images
{
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Images.Models;

    public interface IImageService
    {
        Task<T> CreateImage<T>(IFormFile image, string contentType) where T: IFileData, new();

        Task<ImageListingServiceModel> GetImageByProductId(int productId);
    }
}
