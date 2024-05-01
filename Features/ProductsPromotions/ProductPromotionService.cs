namespace NutriBest.Server.Features.ProductsPromotions
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;

    public class ProductPromotionService : IProductPromotionService
    {
        private readonly NutriBestDbContext db;

        public ProductPromotionService(NutriBestDbContext db)
        {
            this.db = db;
        }

        public async Task<bool> Create(int productId,
            int promotionId)
        {
            var product = await db.Products
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            var promotion = await db.Promotions
                .FirstOrDefaultAsync(x => x.PromotionId == promotionId);

            if (promotion == null || product == null)
                throw new ArgumentNullException("Invalid product/promotion!");

            if (await db.ProductsCategories
                .AnyAsync(x => x.ProductId == productId && x.CategoryId == (int)Data.Enums.Categories.Promotions + 1))
            {
                var promotionCategory = await db.ProductsCategories
                    .FirstAsync(x => x.ProductId == productId && x.CategoryId == (int)Data.Enums.Categories.Promotions + 1);

                promotionCategory.IsDeleted = false;
            }
            else
            {
                db.ProductsCategories.Add(new Data.Models.ProductCategory
                {
                    ProductId = product.ProductId,
                    CategoryId = (int)Data.Enums.Categories.Promotions + 1
                });
            }


            if (product.PromotionId != null)
            {
                if (product.PromotionId == promotionId)
                {
                    throw new InvalidOperationException("The promotion cannot be the same!");
                }

                product.PromotionId = null;
            }

            if (product.Price < promotion.DiscountAmount)
                throw new InvalidOperationException("The price of the product is less than the discount!");

            product.PromotionId = promotionId;

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Remove(int productId)
        {
            var product = await db.Products
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            if (await db.ProductsCategories
                .AnyAsync(x => x.ProductId == productId && x.CategoryId == (int)Data.Enums.Categories.Promotions + 1))
            {
                var promotionCategory = await db.ProductsCategories
                    .FirstAsync(x => x.ProductId == productId && x.CategoryId == (int)Data.Enums.Categories.Promotions + 1);

                promotionCategory.IsDeleted = true;
            }

            if (product == null)
                throw new ArgumentNullException("Invalid product/promotion!");

            product.PromotionId = null;

            await db.SaveChangesAsync();

            return true;
        }
    }
}
