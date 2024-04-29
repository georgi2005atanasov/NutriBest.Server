namespace NutriBest.Server.Features.ProductsDetails
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Images.Models;
    using NutriBest.Server.Features.ProductsDetails.Models;

    public class ProductDetailsService : IProductDetailsService
    {
        private readonly NutriBestDbContext db;

        public ProductDetailsService(NutriBestDbContext db)
        {
            this.db = db;
        }

        public async Task<ProductDetailsServiceModel> GetById(int id)
        {
            var product = await db.Products
                         .Include(x => x.ProductDetails)
                         .Select(x => new ProductDetailsServiceModel
                         {
                             ProductId = x.ProductId,
                             Name = x.Name,
                             Price = x.Price,
                             Categories = x.ProductsCategories
                             .Select(c => c.Category.Name)
                             .ToList(),
                             Description = x.Description,
                             Image = new ImageListingServiceModel
                             {
                                 ContentType = x.ProductImage.ContentType,
                                 ImageData = x.ProductImage.ImageData
                             },
                             Quantity = x.Quantity,
                             HowToUse = x.ProductDetails.HowToUse,
                             ServingSize = x.ProductDetails.ServingSize,
                             ServingsPerContainer = x.ProductDetails.ServingsPerContainer
                         })
                         .FirstAsync(x => x.ProductId == id);

            return product;
        }

        public async Task AddDetails(int productId, string? howToUse, string? servingSize, string? servingsPerContainer)
        {
            var details = await db.ProductsDetails
                .FirstAsync(x => x.ProductId == productId);

            if (!string.IsNullOrEmpty(howToUse))
            {
                details.HowToUse = howToUse;
            }
            if (!string.IsNullOrEmpty(servingSize))
            {
                details.ServingSize = servingSize;
            }
            if (!string.IsNullOrEmpty(servingsPerContainer))
            {
                details.ServingsPerContainer = servingsPerContainer;
            }

            await db.SaveChangesAsync();
        }

        public async Task RemoveDetails(int productId)
        {
            var product = await db.Products
                .FirstAsync(x => x.ProductId == productId);

            var details = await db.ProductsDetails
                .FirstAsync(x => x.ProductId == productId);

            details.HowToUse = "";
            details.ServingSize = "";
            details.ServingsPerContainer = "";

            await db.SaveChangesAsync();
        }
    }
}
