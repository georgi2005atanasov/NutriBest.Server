namespace NutriBest.Server.Features.Images
{
    using NutriBest.Server.Data.Models;

    public class ImageService : IImageService
    {
        public async Task<ProductImage> GetImage(IFormFile image, string contentType)
        {
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);

                var imageByteArray = memoryStream.ToArray();
                var imageData = Convert.ToBase64String(imageByteArray);

                return new ProductImage
                {
                    ImageData = imageData,
                    ContentType = contentType
                };
            }
        }
    }
}
