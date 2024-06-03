using Microsoft.EntityFrameworkCore;
using NutriBest.Server.Data;
using NutriBest.Server.Data.Models;
using NutriBest.Server.Features.Carts.Models;
using NutriBest.Server.Features.Invoices.Models;
using NutriBest.Server.Features.Orders.Models;
using NutriBest.Server.Features.Products.Models;
using NutriBest.Server.Infrastructure.Services;

namespace NutriBest.Server.Features.Orders
{
    public class OrderService : IOrderService
    {
        protected readonly NutriBestDbContext db;
        private readonly ICurrentUserService currentUserService;

        public OrderService(NutriBestDbContext db,
            ICurrentUserService currentUserService)
        {
            this.db = db;
            this.currentUserService = currentUserService;
        }

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

        public async Task<OrderServiceModel?> GetFinishedOrder(int orderId, string? token)
        {
            var orderFromDb = await db.Orders
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (orderFromDb == null)
                return null;

            if (orderFromDb.UserOrderId != null)
            {
                var userOrder = await db.UsersOrders
                    .FirstAsync(x => x.OrderId == orderFromDb.Id);

                if (userOrder.ProfileId != currentUserService.GetUserId())
                {
                    throw new InvalidOperationException();
                }
            }

            if (orderFromDb.GuestOrderId != null)
            {
                if (orderFromDb.SessionToken != token)
                    throw new InvalidOperationException();

                if (currentUserService.GetUserId() != null)
                    throw new InvalidOperationException();
            }

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
                        //productFromDb.Price * ((100 - (decimal)promotion.DiscountAmount) / 100) * cartProduct.Count
                        product.Product.DiscountPercentage = promotion.DiscountAmount * 100 / product.Product.Price;
                    }
                }
            }

            var cartModel = new CartServiceModel
            {
                Code = cart.Code,
                OriginalPrice = cart.OriginalPrice,
                TotalPrice = cart.TotalPrice,
                TotalSaved = cart.TotalSaved,
                CartProducts = cartProducts
            };

            var orderDetails = await db.OrdersDetails
                .FirstAsync(x => x.Id == orderFromDb.OrderDetailsId);

            var order = new OrderServiceModel
            {
                Cart = cartModel,
                IsConfirmed = orderFromDb.IsConfirmed,
                IsFinished = orderFromDb.IsFinished,
                MadeOn = orderDetails.MadeOn,
                PaymentMethod = orderDetails.PaymentMethod.ToString()
            };

            if (orderDetails.InvoiceId != null)
            {
                var invoice = await db.Invoices
                    .FirstAsync(x => x.Id == orderDetails.InvoiceId);

                order.Invoice = new InvoiceServiceModel
                {
                    FirstName = invoice.FirstName,
                    LastName = invoice.LastName,
                    CompanyName = invoice.CompanyName,
                    Bullstat = invoice.Bullstat,
                    PersonInCharge = invoice.PersonInCharge,
                    PhoneNumber = invoice.PhoneNumber,
                    VAT = invoice.VAT
                };
            }

            return order;
        }

        public async Task<bool> ConfirmOrder(int orderId)
        {
            var order = await db.Orders
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
                return false;

            order.IsConfirmed = true;

            await db.SaveChangesAsync();

            return true;
        }
    }
}
