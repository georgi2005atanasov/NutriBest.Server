namespace NutriBest.Server.Features.Promotions
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Categories;
    using NutriBest.Server.Features.Promotions.Models;
    using System.Collections.Generic;

    public class PromotionService : IPromotionService
    {
        private readonly NutriBestDbContext db;
        private readonly IMapper mapper;
        private readonly ICategoryService categoryService;

        public PromotionService(NutriBestDbContext db,
            IMapper mapper,
            ICategoryService categoryService)
        {
            this.db = db;
            this.mapper = mapper;
            this.categoryService = categoryService;
        }

        public async Task<int> Create(string? description,
            decimal? discountAmount,
            decimal? discountPercentage,
            DateTime startDate,
            DateTime? endDate,
            decimal? minPrice,
            string? category)
        {
            if (minPrice <= discountAmount)
            {
                throw new ArgumentException("The minimum price must be bigger that the discount amount!");
            }

            if (await db.Promotions.AnyAsync(x => x.Description == description))
            {
                throw new ArgumentException("Promotion with this description already exists!");
            }

            var productsToApplyPromotion = db.Products
                .AsQueryable();

            var promotion = new Promotion
            {
                StartDate = startDate,
                EndDate = endDate,
                Description = description,
                DiscountAmount = discountAmount,
                DiscountPercentage = discountPercentage,
                IsActive = true,
                MinimumPrice = minPrice,
                Category = category
            };

            db.Promotions.Add(promotion);

            await db.SaveChangesAsync();

            await ApplyPromotion(db, categoryService, promotion);

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
            decimal? discountPercentage,
            decimal? minPrice,
            string? category)
        {
            if (minPrice <= discountAmount)
            {
                throw new ArgumentException("The minimum price must be bigger that the discount amount!");
            }

            if (await db.Promotions.AnyAsync(x => x.Description == description))
            {
                throw new ArgumentException("Promotion with this description already exists!");
            }

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

            if (minPrice != null)
                promotion.MinimumPrice = minPrice;

            if (category != null)
                promotion.Category = category;

            await db.SaveChangesAsync();

            await ApplyPromotion(db, categoryService, promotion);

            return true;
        }

        public async Task<bool> Remove(int promotionId)
        {
            var promotion = await db.Promotions
                .FirstOrDefaultAsync(x => x.PromotionId == promotionId);

            if (promotion == null)
                return false;

            db.Promotions.Remove(promotion);

            var products = db.Products
               .Where(x => x.PromotionId == promotionId)
               .AsQueryable();

            foreach (var product in products)
            {
                product.PromotionId = null;

                var productCategory = await db.ProductsCategories
                .Where(x => x.ProductId == product.ProductId && x.CategoryId ==
                 (int)Data.Enums.Categories.Promotions + 1)
                .FirstAsync();

                db.ProductsCategories.Remove(productCategory);
            }

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

        private static async Task ApplyPromotion(NutriBestDbContext db, ICategoryService categoryService, Promotion promotion)
        {
            var productsToApplyPromotion = db.Products
                .AsQueryable();

            if (promotion.MinimumPrice != null)
            {
                productsToApplyPromotion = productsToApplyPromotion
                    .Where(x => x.Price >= promotion.MinimumPrice);
            }

            if (promotion.Category != null)
            {
                var categoriesIds = await categoryService.GetCategoriesIds(new List<string> { promotion.Category });

                foreach (var product in productsToApplyPromotion.ToList())
                {
                    var categoriesOfProduct = await db.ProductsCategories
                        .Where(x => x.ProductId == product.ProductId)
                        .Select(x => x.CategoryId)
                        .ToListAsync();

                    if (categoriesOfProduct.Contains(categoriesIds[0]))
                    {
                        product.PromotionId = promotion.PromotionId;

                        if (!await db.ProductsCategories
                            .AnyAsync(x => x.ProductId == product.ProductId &&
                            x.CategoryId == (int)Data.Enums.Categories.Promotions + 1))
                        {
                            db.ProductsCategories.Add(new Data.Models.ProductCategory
                            {
                                ProductId = product.ProductId,
                                CategoryId = (int)Data.Enums.Categories.Promotions + 1
                            });
                        }
                    }
                }
            }

            await db.SaveChangesAsync();
        }
    }
}