namespace NutriBest.Server.Features.Promotions
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Categories;
    using NutriBest.Server.Features.Products.Models;
    using NutriBest.Server.Features.ProductsPromotions;
    using NutriBest.Server.Features.Promotions.Models;
    using System.Collections.Generic;

    public class PromotionService : IPromotionService
    {
        private readonly NutriBestDbContext db;
        private readonly IMapper mapper;
        private readonly IProductPromotionService productPromotionService;
        private readonly ICategoryService categoryService;

        public PromotionService(NutriBestDbContext db,
            IMapper mapper,
            IProductPromotionService productPromotionService,
            ICategoryService categoryService)
        {
            this.db = db;
            this.mapper = mapper;
            this.productPromotionService = productPromotionService;
            this.categoryService = categoryService;
        }

        public async Task<int> Create(string? description,
            decimal? discountAmount,
            decimal? discountPercentage,
            DateTime startDate,
            DateTime? endDate,
            decimal? minPrice,
            string? category,
            string? brandName)
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
                MinimumPrice = minPrice,
                Category = category
            };

            if (brandName != null)
            {
                var brand = await db.Brands
                    .FirstOrDefaultAsync(x => x.Name == brandName);

                if (brand == null)
                    throw new InvalidOperationException("Invalid brand!");

                promotion.Brand = brandName;
            }

            promotion.IsActive = false;
            //promotion.IsActive = startDate <= DateTime.UtcNow;

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
            string? category,
            string? brandName,
            DateTime? startDate,
            DateTime? endDate)
        {
            if (minPrice <= discountAmount)
            {
                throw new ArgumentException("The minimum price must be bigger that the discount amount!");
            }

            if (await db.Promotions.AnyAsync(x => x.Description == description && x.PromotionId != promotionId))
            {
                throw new ArgumentException("Promotion with this description already exists!");
            }

            var promotion = await db.Promotions
                .FirstOrDefaultAsync(x => x.PromotionId == promotionId);

            if (promotion == null)
                throw new InvalidOperationException("Promotion does not exist!");

            if (await db.Products.AnyAsync(x => x.PromotionId == promotionId && discountAmount != null && x.Price <= discountAmount || (minPrice != null && x.Price < minPrice)))
            {
                var possiblePrice = await db.Products
                    .Where(x => x.PromotionId == promotionId && x.Price <= discountAmount || x.Price < minPrice)
                    .OrderBy(x => x.Price)
                    .FirstOrDefaultAsync();

                throw new ArgumentException($"The discount cannot be applied to all the products!");
            }

            if (discountAmount != null && promotion.DiscountPercentage != null)
            {
                promotion.DiscountPercentage = null;
                //throw new InvalidOperationException("You can only change the discount percentage!");
            }

            if (discountPercentage != null && promotion.DiscountAmount != null)
            {
                promotion.DiscountAmount = null;
                //throw new InvalidOperationException("You can only change the discount amount!");
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

            if (endDate != null)
            {
                promotion.EndDate = endDate;
            }

            if (brandName != null)
            {
                var brand = await db.Brands
                    .FirstOrDefaultAsync(x => x.Name == brandName);

                if (brand == null)
                    throw new InvalidOperationException("Invalid brand!");

                promotion.Brand = brandName;
            }

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

            promotion.IsActive = false;

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
            if (!promotion.IsActive)
            {
                var productsToChange = db.Products
                    .Where(x => x.PromotionId == promotion.PromotionId);

                //the code below is copied from the productPromotionService
                foreach (var product in productsToChange)
                {
                    if (await db.ProductsCategories
                        .AnyAsync(x => x.ProductId == product.ProductId &&
                        x.CategoryId == (int)Data.Enums.Categories.Promotions + 1))
                    {
                        var promotionCategory = await db.ProductsCategories
                            .FirstAsync(x => x.ProductId == product.ProductId && x.CategoryId == (int)Data.Enums.Categories.Promotions + 1);

                        db.ProductsCategories.Remove(promotionCategory);
                    }

                    product.PromotionId = null;
                }

                return;
            }

            var productsToApplyPromotion = db.Products
                .AsQueryable();

            if (promotion.MinimumPrice != null)
                productsToApplyPromotion = productsToApplyPromotion
                    .Where(x => x.Price >= promotion.MinimumPrice);

            if (promotion.Category != null)
            {
                var categoriesIds = await categoryService.GetCategoriesIds(new List<string> { promotion.Category });

                foreach (var product in productsToApplyPromotion.ToList()) //idk i make this to be a list
                {
                    if ((promotion.MinimumPrice != null && 
                        product.Price >= promotion.MinimumPrice) ||
                        promotion.MinimumPrice == null)
                    {
                        var categoriesOfProduct = await db.ProductsCategories
                            .Where(x => x.ProductId == product.ProductId)
                            .Select(x => x.CategoryId)
                            .ToListAsync();

                        if (!categoriesOfProduct.Contains(categoriesIds[0]))
                            continue;

                        var brand = await db.Brands
                                .FirstAsync(x => x.Name == promotion.Brand);

                        if (brand.Id != product.BrandId)
                            continue;

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

        public async Task<bool> ChangeIsActive(int promotionId)
        {
            var promotion = await db.Promotions
                .FirstOrDefaultAsync(x => x.PromotionId == promotionId);

            if (promotion == null)
            {
                throw new ArgumentNullException("The promotion is not valid!");
            }

            if ((promotion.EndDate != null && promotion.EndDate < DateTime.UtcNow) ||
                promotion.StartDate > DateTime.UtcNow)
            {
                throw new InvalidOperationException("You cannot change the status of the promotion!");
            }

            promotion.IsActive = !promotion.IsActive;

            await ApplyPromotion(db, categoryService, promotion);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<List<ProductServiceModel>> GetProductsOfPromotion(int promotionId)
        {
            var promotion = await db.Promotions
                .Where(x => x.IsActive)
                .FirstOrDefaultAsync(x => x.PromotionId == promotionId);

            if (promotion == null)
                return new List<ProductServiceModel>();

            var products = await db.Products
                .Where(x => x.PromotionId == promotionId)
                .ProjectTo<ProductServiceModel>(mapper.ConfigurationProvider)
                .ToListAsync();

            return products;
        }
    }
}