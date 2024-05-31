using Microsoft.EntityFrameworkCore;
using NutriBest.Server.Data;
using NutriBest.Server.Data.Models;
using NutriBest.Server.Features.Carts.Models;
using NutriBest.Server.Features.Orders.Models;
using NutriBest.Server.Features.Products.Models;

namespace NutriBest.Server.Features.Orders
{
    public class OrderService : IOrderService
    {
        protected readonly NutriBestDbContext db;

        public OrderService(NutriBestDbContext db)
            => this.db = db;

        public async Task<int> PrepareCart(decimal totalPrice,
           decimal originalPrice,
           decimal totalSaved,
           string? code,
           List<CartProductServiceModel> cartProducts)
        {
            var cart = new Cart
            {
                TotalPrice = totalPrice,
                OriginalPrice = originalPrice,
                TotalSaved = totalSaved,
                Code = code
            };

            foreach (var cartProductModel in cartProducts)
            {
                var product = await db.Products
                    .FirstAsync(x => x.ProductId == cartProductModel.ProductId);

                var package = await db.Packages
                    .FirstAsync(x => x.Grams == cartProductModel.Grams);

                var flavour = await db.Flavours
                    .FirstAsync(x => x.FlavourName == cartProductModel.Flavour);

                var cartProduct = new CartProduct
                {
                    Cart = cart,
                    Count = cartProductModel.Count,
                    Product = product,
                    ProductId = product.ProductId,
                    Package = package,
                    PackageId = package.Id,
                    Flavour = flavour,
                    FlavourId = flavour.Id,
                };

                db.CartProducts.Add(cartProduct);
            }

            await db.SaveChangesAsync();

            return cart.Id;
        }

        public async Task<CartServiceModel?> GetFinishedOrder(int orderId)
        {
            var orderFromDb = await db.Orders
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (orderFromDb == null)
                return null;

            var cart = await db.Carts
                .FirstAsync(x => x.Id == orderFromDb.CartId);

            var cartProducts = await db.CartProducts
                .Where(x => x.CartId == orderFromDb.CartId)
                .Select(x => new CartProductServiceModel
                {
                    Count = x.Count,
                    Grams = db.Packages.First(y => y.Id == x.PackageId).Grams,
                    Flavour = db.Flavours.First(y => y.Id == x.FlavourId).FlavourName,
                    ProductId = x.ProductId,
                    Product = new ProductListingServiceModel
                    {
                        ProductId = x.Product!.ProductId,
                        Name = x.Product.Name,
                        Price = x.Product.Price,
                        Categories = x.Product.ProductsCategories
                             .Select(c => c.Category.Name)
                             .ToList(),
                        Quantity = x.Product.Quantity,
                        PromotionId = x.Product.PromotionId,
                        Brand = x.Product.Brand!.Name // be aware
                    }
                })
                .ToListAsync();

            var cartModel = new CartServiceModel
            {
                Code = cart.Code,
                OriginalPrice = cart.OriginalPrice,
                TotalPrice = cart.TotalPrice,
                TotalSaved = cart.TotalSaved,
                CartProducts = cartProducts
            };

            var order = new OrderServiceModel
            {
                Cart = cartModel
            };

            return cartModel;
        }

    }
}
