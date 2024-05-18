namespace NutriBest.Server.Features.Carts
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Carts.Models;
    using NutriBest.Server.Features.Products.Models;
    using NutriBest.Server.Features.Promotions.Models;
    using NutriBest.Server.Infrastructure.Services;

    public class CartService : ICartService
    {
        private readonly NutriBestDbContext db;
        private readonly ICurrentUserService currentUser;
        private readonly IMapper mapper;

        public CartService(NutriBestDbContext db,
            ICurrentUserService currentUser,
            IMapper mapper)
        {
            this.db = db;
            this.currentUser = currentUser;
            this.mapper = mapper;
        }

        public async Task<CartServiceModel> Get()
        {
            var (userId, profile, cart, cartProducts) = await GetDataForChangingCart();

            var products = await cartProducts
                .Select(x => new CartProductServiceModel
                {
                    ProductId = x.ProductId,
                    Count = x.Count
                })
                .ToListAsync();

            foreach (var cartProduct in products)
            {
                var product = await db.Products
                    .Select(x => new ProductListingServiceModel
                    {
                        ProductId = x.ProductId,
                        Name = x.Name,
                        Price = x.Price,
                        Quantity = x.Quantity,
                        PromotionId = x.PromotionId,
                        Brand = x.Brand!.Name, // be aware
                    })
                    .FirstAsync(x => x.ProductId == cartProduct.ProductId);

                if (product.PromotionId != null)
                {
                    var promotion = await db.Promotions
                        .FirstAsync(x => x.PromotionId == product.PromotionId);
                }

                cartProduct.Product = product;
            }

            var cartModel = new CartServiceModel
            {
                CartProducts = products,
                TotalPrice = cart.TotalPrice
            };

            return cartModel;
        }

        public async Task<int?> Add(int productId, int count)
        {
            var (userId, profile, cart, cartProducts) = await GetDataForChangingCart();

            CartProduct cartProduct;

            if (await cartProducts.AnyAsync(x => x.ProductId == productId))
            {
                cartProduct = await db.CartProducts
                    .FirstAsync(x => x.ProductId == productId);

                cartProduct.Count += count;

                db.CartProducts.Update(cartProduct);
            }
            else
            {
                cartProduct = new CartProduct
                {
                    CartId = (int)profile!.CartId!,
                    ProductId = productId,
                    Count = count
                };

                db.CartProducts.Add(cartProduct);
            }

            var product = await db.Products
                    .FirstAsync(x => x.ProductId == productId);

            if (cartProduct.Count > product.Quantity)
            {
                throw new InvalidOperationException($"Sorry, we have {product.Quantity} units of this product available.");
            }

            cart.TotalPrice += (product.Price + 0.99m) * count;

            await db.SaveChangesAsync();

            return cartProduct.Id;
        }

        public async Task<bool> Remove(int productId, int count)
        {
            var (userId, profile, cart, cartProducts) = await GetDataForChangingCart();

            CartProduct cartProduct;

            if (await cartProducts.AnyAsync(x => x.ProductId == productId))
            {
                cartProduct = await db.CartProducts
                    .FirstAsync(x => x.ProductId == productId);

                if (cartProduct.Count - count == 0)
                {
                    db.CartProducts.Remove(cartProduct);
                }
                else
                {
                    cartProduct.Count -= count;

                    db.CartProducts.Update(cartProduct);
                }
            }
            else
            {
                return false;
            }

            var product = await db.Products
                    .FirstAsync(x => x.ProductId == productId);

            cart.TotalPrice -= (product.Price + 0.99m) * count;

            db.Carts.Update(cart);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Clean()
        {
            var (userId, profile, cart, cartProducts) = await GetDataForChangingCart();

            if (cartProducts.Any() && cart.TotalPrice == 0)
            {
                return false;
            }

            db.CartProducts.RemoveRange(cartProducts);

            cart.TotalPrice = 0;
            db.Carts.Update(cart);

            await db.SaveChangesAsync();

            return true;
        }

        private async Task<(string? userId,
            Data.Models.Profile? profile,
            Cart cart,
            IQueryable<CartProduct> cartProducts)> GetDataForChangingCart()
        {
            var userId = currentUser.GetUserId();

            var profile = await db.Profiles
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();

            var cart = await db.Carts
                .FirstAsync(x => x.Id == profile!.CartId);

            var cartProducts = db.CartProducts
                .AsQueryable();

            return (
                userId,
                profile,
                cart,
                cartProducts
                );
        }
    }
}
