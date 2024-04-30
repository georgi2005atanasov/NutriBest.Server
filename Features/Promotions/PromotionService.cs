namespace NutriBest.Server.Features.Promotions
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Promotions.Models;

    public class PromotionService : IPromotionService
    {
        private readonly NutriBestDbContext db;

        public PromotionService(NutriBestDbContext db)
        {
            this.db = db;
        }

        public async Task<bool> CreateProductPromotion(int productId, //new shit
            int promotionId,
            decimal? specialPrice)
        {
            var productPromotion = new ProductPromotion
            {
                ProductId = productId,
                PromotionId = promotionId
            };

            if (specialPrice != null)
            {
                productPromotion.SpecialPrice = specialPrice;
            }

            db.ProductsPromotions.Add(productPromotion);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<int> Create(string? description,
            decimal? discountAmount,
            decimal? discountPercentage,
            DateTime startDate,
            DateTime endDate,
            decimal? specialPrice)
        {
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

        public async Task<bool> Update(int promotionId, string? description, decimal? discountAmount, decimal? discountPercentage, decimal? specialPrice)
        {
            var promotion = await db.Promotions
                .FirstOrDefaultAsync(x => x.PromotionId == promotionId);

            if (promotion == null)
            {
                throw new InvalidOperationException("Product does not exist!");
            }

            if (string.IsNullOrEmpty(description))
            {

            }
            promotion.Description = description;
            return true;
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