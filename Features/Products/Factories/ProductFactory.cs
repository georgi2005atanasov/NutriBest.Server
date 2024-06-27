namespace NutriBest.Server.Features.Products.Factories
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Products.Models;

    public class ProductFactory
    {
        public static async Task<OrderRelatedProductServiceModel> CreateOrderRelatedProductModelAsync(NutriBestDbContext db,
            ProductPackageFlavour productPackageFlavour)
        {
            var product = await db.Products
                        .FirstAsync(x => x.ProductId == productPackageFlavour.ProductId);

            var flavour = await db.Flavours
                .FirstAsync(x => x.Id == productPackageFlavour.FlavourId);

            var package = await db.Packages
                .FirstAsync(x => x.Id == productPackageFlavour.PackageId);

            var promotion = await db.Promotions
                .FirstOrDefaultAsync(x => x.PromotionId == product.PromotionId);

            decimal discountPercentage = 0;

            if (promotion != null && promotion.DiscountPercentage != null)
            {
                discountPercentage = (decimal)promotion.DiscountPercentage;
            }
            else if (promotion != null && promotion.DiscountAmount != null)
            {
                discountPercentage = (decimal)promotion.DiscountAmount * 100 / productPackageFlavour.Price; ;
            }

            var productModel = new OrderRelatedProductServiceModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = productPackageFlavour.Price,
                Flavour = flavour.FlavourName,
                Grams = package.Grams,
                Quantity = productPackageFlavour.Quantity,
                PromotionId = product.PromotionId,
                DiscountPercentage = discountPercentage
            };

            return productModel;
        }

        public static async Task<ProductSpecsServiceModel> CreateProductSpecsServiceModelAsync(NutriBestDbContext db,
            ProductPackageFlavour productPackageFlavour)
        {
            var package = await db.Packages
                    .FirstAsync(x => x.Id == productPackageFlavour.PackageId);

            var flavour = await db.Flavours
                .FirstAsync(x => x.Id == productPackageFlavour.FlavourId);

            var spec = new ProductSpecsServiceModel
            {
                Flavour = flavour.FlavourName,
                Grams = package.Grams,
                Quantity = productPackageFlavour.Quantity,
                Price = $"{productPackageFlavour.Price}"
            };

            return spec;
        }
    }
}
