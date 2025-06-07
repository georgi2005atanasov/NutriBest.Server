namespace NutriBest.Server.Features.Images
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Images.Models;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;

    public class ImageService : IImageService, ITransientService
    {
        private readonly NutriBestDbContext db;

        public ImageService(NutriBestDbContext db)
            => this.db = db;

        public async Task<T> CreateImage<T>(IFormFile image, string contentType) where T : IFileData, new()
        {
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);

                var imageByteArray = memoryStream.ToArray();
                var imageData = Convert.ToBase64String(imageByteArray);

                return new T
                {
                    ImageData = imageData,
                    ContentType = contentType
                };
            }
        }

        // may be able to make single method for getting images using reflection
        public async Task<ImageListingServiceModel> GetImageByBrandId(string name)
        {
            var brand = await db.Brands
                .FirstOrDefaultAsync(x => x.Name == name);

            if (brand == null)
                return null!;

            var image = await db.BrandsLogos
                .Where(x => x.Brand == brand)
                .Select(x => new ImageListingServiceModel
                {
                    ContentType = x.ContentType,
                    ImageData = x.ImageData,

                })
                .FirstOrDefaultAsync();

            return image!;
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
