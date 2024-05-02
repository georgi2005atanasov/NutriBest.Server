namespace NutriBest.Server.Features.Promotions
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Promotions.Models;
    using System.Collections.Generic;

    public class PromotionService : IPromotionService
    {
        private readonly NutriBestDbContext db;
        private readonly IMapper mapper;

        public PromotionService(NutriBestDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<int> Create(string? description,
            decimal? discountAmount,
            decimal? discountPercentage,
            DateTime startDate,
            DateTime endDate)
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
                .ProjectTo<PromotionServiceModel>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (promotion == null)
                throw new ArgumentNullException("The promotion is not valid!");

            return promotion;
        }

        public async Task<bool> Update(int promotionId, 
            string? description, 
            decimal? discountAmount, 
            decimal? discountPercentage)
        {
            var promotion = await db.Promotions
                .FirstOrDefaultAsync(x => x.PromotionId == promotionId);

            if (await db.Products.AnyAsync(x => x.PromotionId == promotionId && discountAmount != null && x.Price <= discountAmount))
            {
                var possiblePrice = await db.Products
                    .Where(x => x.PromotionId == promotionId && x.Price <= discountAmount)
                    .OrderBy(x => x.Price)
                    .FirstAsync();

                throw new ArgumentException($"Promotion price cannot be higher than {possiblePrice.Price - 0.1m}");
            }

            if (promotion == null)
                throw new InvalidOperationException("Promotion does not exist!");

            if (discountAmount != null && promotion.DiscountPercentage != null)
            {
                throw new InvalidOperationException("You can only change the discount percentage!");
            }

            if (discountPercentage != null && promotion.DiscountAmount != null)
            {
                throw new InvalidOperationException("You can only change the discount amount!");
            }

            if (!string.IsNullOrEmpty(description))
                promotion.Description = description;

            if (discountAmount != null)
                promotion.DiscountAmount = discountAmount;

            if (discountPercentage != null)
                promotion.DiscountPercentage = discountPercentage;

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Remove(int promotionId)
        {
            var promotion = await db.Promotions
                .FirstOrDefaultAsync(x => x.PromotionId == promotionId);

            if (promotion == null)
                return false;

            db.Promotions.Remove(promotion);

            await db.Products
               .Where(x => x.PromotionId == promotionId)
               .ForEachAsync(x =>
               {
                   x.PromotionId = null;
               });

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<PromotionServiceModel>> All()
        {
            var promotions = await db.Promotions
                .ProjectTo<PromotionServiceModel>(mapper.ConfigurationProvider)
                .ToListAsync();

            return promotions;
        }
    }
}