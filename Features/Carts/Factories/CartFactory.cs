namespace NutriBest.Server.Features.Carts.Factories
{
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Features.Carts.Models;
    using NutriBest.Server.Features.Products.Models;

    public static class CartFactory
    {
        public static async Task<CartServiceModel> CreateCartServiceModelAsync(Cart cart,
            NutriBestDbContext db,
            string customerName,
            string customerEmail,
            string phoneNumber)
        {
            var cartProducts = await db.CartProducts
                .Where(x => x.CartId == cart.Id)
                .Select(x => new CartProductServiceModel
                {
                    Count = x.Count,
                    Grams = db.Packages.First(y => y.Id == x.PackageId).Grams,
                    Flavour = db.Flavours.First(y => y.Id == x.FlavourId).FlavourName,
                    ProductId = x.ProductId,
                    Price = db.ProductsPackagesFlavours
                        .First(y => y.FlavourId == x.FlavourId &&
                        y.PackageId == x.PackageId &&
                        y.ProductId == x.ProductId)
                        .Price,
                    Product = new ProductListingServiceModel
                    {
                        ProductId = x.Product!.ProductId,
                        Name = x.Product.Name,
                        Price = x.Product
                        .ProductPackageFlavours
                        .First(y => y.PackageId == x.PackageId &&
                        y.ProductId == x.Product.ProductId &&
                        y.FlavourId == x.FlavourId).Price,
                        Categories = x.Product.ProductsCategories
                             .Select(c => c.Category.Name)
                             .ToList(),
                        Quantity = x.Product.Quantity,
                        PromotionId = x.Product.PromotionId,
                        Brand = x.Product.Brand!.Name, // be aware,
                    }
                })
                .ToListAsync();

            await GetDiscountPercentageForTheProducts(db, cartProducts);

            var cartModel = new CartServiceModel
            {
                Code = cart.Code,
                OriginalPrice = cart.OriginalPrice,
                TotalProducts = cart.TotalProducts,
                TotalSaved = cart.TotalSaved,
                CartProducts = cartProducts,
                ShippingPrice = cart.ShippingPrice // added this
            };

            return cartModel;
        }

        private static async Task GetDiscountPercentageForTheProducts(NutriBestDbContext db, List<CartProductServiceModel> cartProducts)
        {
            foreach (var product in cartProducts)
            {
                if (product.Product!.PromotionId != null)
                {
                    var promotion = await db.Promotions
                        .FirstAsync(x => x.PromotionId == product.Product.PromotionId);

                    if (promotion.DiscountPercentage != null)
                    {
                        product.Product.DiscountPercentage = promotion.DiscountPercentage;
                    }

                    if (promotion.DiscountAmount != null)
                    {
                        product.Product.DiscountPercentage = promotion.DiscountAmount * 100 / product.Product.Price;
                    }
                }
            }
        }
    }
}
