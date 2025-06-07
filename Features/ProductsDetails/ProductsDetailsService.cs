using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.ProductsDetails
{
    using Microsoft.EntityFrameworkCore;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Promotions;
    using NutriBest.Server.Features.ProductsDetails.Models;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;
    using static ErrorMessages.ProductsController;

    public class ProductsDetailsService : IProductsDetailsService, ITransientService
    {
        private readonly NutriBestDbContext db;
        private readonly IPromotionService promotionService;
        private readonly IMapper mapper;

        public ProductsDetailsService(NutriBestDbContext db,
            IPromotionService promotionService,
            IMapper mapper)
        {
            this.db = db;
            this.promotionService = promotionService;
            this.mapper = mapper;
        }

        public async Task<ProductDetailsServiceModel> GetById(int id)
        {
            var product = await db.Products
                         .Include(x => x.ProductDetails)
                         .ProjectTo<ProductDetailsServiceModel>(mapper.ConfigurationProvider)
                         .FirstOrDefaultAsync(x => x.ProductId == id);

            if (product == null)
                throw new ArgumentNullException(InvalidProduct);

            await GetPromotionPercentage(product);

            return product;
        }

        public async Task AddDetails(int productId,
            string? howToUse,
            string? servingSize,
            string? whyChoose,
            string? ingredients)
        {
            var details = await db.ProductsDetails
                .FirstAsync(x => x.ProductId == productId);

            if (!string.IsNullOrEmpty(howToUse))
                details.HowToUse = howToUse;

            if (!string.IsNullOrEmpty(servingSize))
                details.ServingSize = servingSize;

            if (!string.IsNullOrEmpty(whyChoose))
                details.WhyChoose = whyChoose;

            if (!string.IsNullOrEmpty(ingredients))
                details.Ingredients = ingredients;

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
            details.WhyChoose = "";
            details.Ingredients = "";

            await db.SaveChangesAsync();
        }

        private async Task GetPromotionPercentage(ProductDetailsServiceModel product)
        {
            if (product.PromotionId != null)
            {
                try
                {
                    var promotion = await promotionService.Get((int)product.PromotionId);

                    if (!promotion.IsActive)
                    {
                        throw new Exception();
                    }

                    if (promotion != null && promotion.DiscountPercentage != null)
                    {
                        product.DiscountPercentage = promotion.DiscountPercentage;
                    }

                    if (promotion != null && promotion.DiscountAmount != null)
                    {
                        product.DiscountPercentage = promotion.DiscountAmount * 100 / product.Price;
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
        }
    }
}

