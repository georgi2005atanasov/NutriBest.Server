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
            var product = db.Products
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
            };

            db.Promotions.Add(promotion);

            var productPromotion = new ProductPromotion
            {
                ProductId = productId,
                PromotionId = promotion.PromotionId
            };

            if (specialPrice != null)
            {
                productPromotion.SpecialPrice = specialPrice;
            }

            var id = db.ProductsPromotions.Add(productPromotion);

            await db.SaveChangesAsync();

            return productPromotion.PromotionId;
        }

        public async Task<PromotionServiceModel> Get()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Remove()
        {
            throw new NotImplementedException();
        }
    }
}
