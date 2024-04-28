namespace NutriBest.Server.Features.ProductsDetails
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    
    public class ProductDetailsService : IProductDetailsService
    {
        private readonly NutriBestDbContext db;

        public ProductDetailsService(NutriBestDbContext db)
        {
            this.db = db;
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

        public async Task RemoveDetails(int productId, string name)
        {
            var product = await db.Products
                .FirstAsync(x => x.ProductId == productId);

            if (product.Name != name)
                throw new InvalidOperationException("Invalid product!");

            var details = await db.ProductsDetails
                .FirstAsync(x => x.ProductId == productId);

            details.HowToUse = "";
            details.ServingSize = "";
            details.ServingsPerContainer = "";

            await db.SaveChangesAsync();
        }
    }
}
