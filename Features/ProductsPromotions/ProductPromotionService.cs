namespace NutriBest.Server.Features.ProductsPromotions
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Categories;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;

    public class ProductPromotionService : IProductPromotionService, ITransientService
    {
        private readonly NutriBestDbContext db;
        private readonly ICategoryService categoryService;

        public ProductPromotionService(NutriBestDbContext db,
            ICategoryService categoryService)
        {
            this.db = db;
            this.categoryService = categoryService;
        }

        public async Task<bool> Create(int productId, int promotionId)
        {
            var (product, promotion) = await ValidateProductAndPromotion(productId, promotionId);
            await ValidateProductCategory(productId, promotion.Category);
            await ValidateBrand(product, promotion.Brand);
            ValidatePromotionPrice(product, promotion.DiscountAmount);

            if (!await db.ProductsCategories
                .AnyAsync(x => x.ProductId == productId && x.CategoryId == (int)Data.Enums.Categories.Promotions + 1))
            {
                db.ProductsCategories.Add(new Data.Models.ProductCategory
                {
                    ProductId = product.ProductId,
                    CategoryId = (int)Data.Enums.Categories.Promotions + 1
                });
            }

            if (product.PromotionId != null && product.PromotionId == promotionId)
                throw new InvalidOperationException("The promotion cannot be the same!");

            product.PromotionId = promotionId;

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Remove(int productId)
        {
            var product = await db.Products.FirstOrDefaultAsync(x => x.ProductId == productId);

            if (product == null)
                throw new ArgumentNullException("Invalid product ID!");

            if (await db.ProductsCategories
                .AnyAsync(x => x.ProductId == productId && x.CategoryId == (int)Data.Enums.Categories.Promotions + 1))
            {
                var promotionCategory = await db.ProductsCategories
                    .FirstAsync(x => x.ProductId == productId && x.CategoryId == (int)Data.Enums.Categories.Promotions + 1);

                db.ProductsCategories.Remove(promotionCategory);
            }

            product.PromotionId = null;

            await db.SaveChangesAsync();

            return true;
        }

        private async Task ValidateBrand(Product product, string? brandName)
        {
            if (brandName == null)
                return;

            var brand = await db.Brands
                .FirstAsync(x => x.Name == brandName);

            if (brand.Id != product.BrandId)
                throw new InvalidOperationException("The promotion cannot be applied due to its Brand!");
        }

        private async Task<(Product product, Promotion promotion)> ValidateProductAndPromotion(int productId, int promotionId)
        {
            var product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            var promotion = await db.Promotions.FirstOrDefaultAsync(p => p.PromotionId == promotionId);

            if (product == null || promotion == null)
                throw new ArgumentNullException("Invalid product or promotion!");

            if (!promotion.IsActive)
            {
                throw new ArgumentException("The promotion is not active!");
            }

            return (product, promotion);
        }

        private async Task ValidateProductCategory(int productId, string? category)
        {
            var promotionCategoryId = await categoryService.GetCategoriesIds(new List<string> { category ?? "" });
            if (promotionCategoryId.Count > 0)
            {
                bool isProductInCategory = await db.ProductsCategories
                    .AnyAsync(pc => pc.ProductId == productId && pc.CategoryId == promotionCategoryId[0]);
                if (!isProductInCategory)
                    throw new InvalidOperationException($"The product is not of category {category}");
            }
        }

        private void ValidatePromotionPrice(Product product, decimal? discountAmount)
        {
            if (discountAmount != null && product.StartingPrice <= discountAmount)
                throw new InvalidOperationException("The price of the product must be bigger than the discount!");
        }
    }
}
