namespace NutriBest.Server.Features.Images
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Images.Models;

    public class ImageService : IImageService
    {
        private readonly NutriBestDbContext db;

        public ImageService(NutriBestDbContext db)
            => this.db = db;

        public async Task<ProductImage> CreateImage(IFormFile image, string contentType)
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

        public async Task<ImageListingServiceModel> GetImageByProductId(int productId)
        {
            var product = await db.Products
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            if (product == null)
                return null!;

            var image = await db.ProductsImages
                .Where(x => x.ProductImageId == product.ProductImageId)
                .Select(x => new ImageListingServiceModel
                {
                    ContentType = x.ContentType,
                    ImageData = x.ImageData,
                    
                })
                .FirstOrDefaultAsync();

            return image!;
        }
    }
}
