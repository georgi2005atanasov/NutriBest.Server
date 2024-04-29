namespace NutriBest.Server.Features.Promotions
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Promotions.Models;

    public class ProductPromotionService : IProductPromotionService
    {
        private readonly NutriBestDbContext db;

        public ProductPromotionService(NutriBestDbContext db)
        {
            this.db = db;
        }

        public async Task<int> Create(int productId,
            string? description,
            decimal? discountAmount,
            decimal? discountPercentage,
            DateTime startDate,
            DateTime endDate,
            decimal? specialPrice)
        {
            var product = await db.Products
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            if (product == null)
            {
                throw new InvalidOperationException("Invalid product!");
            }

            var promotion = new Promotion
            {
                StartDate = startDate,
                EndDate = endDate,
                Description = description,
                DiscountAmount = discountAmount,
                DiscountPercentage = discountPercentage,
                IsActive = true,
            };

            db.Promotions.Add(promotion);

            await db.SaveChangesAsync();

            var productPromotion = new ProductPromotion
            {
                ProductId = productId,
                PromotionId = promotion.PromotionId
            };

            if (specialPrice != null)
            {
                productPromotion.SpecialPrice = specialPrice;
            }

            db.ProductsPromotions.Add(productPromotion);

            await db.SaveChangesAsync();

            return promotion.PromotionId;
        }

        public async Task<PromotionServiceModel> Get(int promotionId)
        {
            var promotion = await db.Promotions
                .Where(x => x.PromotionId == promotionId)
                .Select(x => new PromotionServiceModel
                {
                    Description = x.Description,
                    DiscountAmount = x.DiscountAmount,
                    DiscountPercentage = x.DiscountPercentage,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsActive = x.StartDate < x.EndDate
                })
                .FirstOrDefaultAsync();

            if (promotion == null)
            {
                throw new InvalidOperationException("The promotion is not valid!");
            }

            var productPromotions = await db.ProductsPromotions
                .Where(x => x.PromotionId == promotionId)
                .FirstAsync();

            promotion.SpecialPrice = productPromotions.SpecialPrice;

            return promotion;
        }

        public async Task<bool> Remove(int promotionId)
        {
            var promotion = await db.Promotions
                .FirstOrDefaultAsync(x => x.PromotionId == promotionId);

            if (promotion == null)
            {
                return false;
            }

            db.Promotions.Remove(promotion);

            await db.ProductsPromotions
               .Where(x => x.PromotionId == promotionId)
               .ForEachAsync(x =>
               {
                   x.IsDeleted = true;
               });

            await db.SaveChangesAsync();

            return true;
        }
    }
}
