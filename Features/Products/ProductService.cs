using NutriBest.Server.Data;
using NutriBest.Server.Data.Models;

namespace NutriBest.Server.Features.Products
{
    public class ProductService : IProductService
    {
        private readonly NutriBestDbContext db;

        public ProductService(NutriBestDbContext db) 
            => this.db = db;

        public async Task<int> Create(string name, 
            string description, 
            decimal price, 
            byte[] imageData, 
            string contentType)
        {
            var productImage = new ProductImage
            {
                ImageData = imageData,
                ContentType = contentType
            };

            var product = new Product
            {
                Name = name,
                Description = description,
                Price = price,
                ProductImage = productImage
            };

            db.Products.Add(product);
            await db.SaveChangesAsync();

            return product.ProductId;
        }

        public async Task<ProductImage> GetImage(IFormFile image, string contentType)
        {
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);

                return new ProductImage
                {
                    ImageData = memoryStream.ToArray(),
                    ContentType = contentType
                };
            }
        }
    }
}
