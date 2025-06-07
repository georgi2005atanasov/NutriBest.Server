using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Orders
{
    using Microsoft.EntityFrameworkCore;
    using AutoMapper;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Carts.Models;
    using NutriBest.Server.Features.Orders.Models;
    using NutriBest.Server.Infrastructure.Services;
    using NutriBest.Server.Features.Orders.Extensions;
    using NutriBest.Server.Features.Notifications;
    using NutriBest.Server.Features.Orders.Factories;
    using NutriBest.Server.Features.Carts.Factories;
    using NutriBest.Server.Features.Products.Factories;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;
    using static SuccessMessages.NotificationService;
    using static ErrorMessages.OrdersController;

    public class OrderService : IOrderService, ITransientService
    {
        protected readonly NutriBestDbContext db;
        private readonly ICurrentUserService currentUserService;
        private readonly IConfiguration config;
        private readonly INotificationService notificationService;
        private readonly IMapper mapper;
        Func<NutriBestDbContext, OrderDetails, Task<(Address address, City city, Country country)>> getAddressCityCountryFunc;
        Func<NutriBestDbContext, int, Task<OrderDetails>> getOrderDetailsFunc;

        public OrderService(NutriBestDbContext db,
            ICurrentUserService currentUserService,
            IConfiguration config,
            INotificationService notificationService,
            IMapper mapper)
        {
            this.db = db;
            this.currentUserService = currentUserService;
            this.config = config;
            this.notificationService = notificationService;
            this.mapper = mapper;
            this.getAddressCityCountryFunc = this.GetAddressCityCountry;
            this.getOrderDetailsFunc = this.GetOrderDetails;
        }

        public async Task<int> PrepareCart(decimal totalProducts,
           decimal originalPrice,
           decimal totalSaved,
           string? code,
           List<CartProductServiceModel> cartProducts)
        {
            var cart = new Cart
            {
                TotalProducts = totalProducts,
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

        public async Task<AllOrdersServiceModel> All(int page, 
            string? search, 
            string? filters,
            DateTime? startDate,
            DateTime? endDate)
        {
            var orders = db.Orders
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.OrderDetails!.MadeOn)
                .AsQueryable();

            var allOrders = new AllOrdersServiceModel();

            foreach (var order in orders)
            {
                var orderListingModel = await OrderFactory.CreateOrderListingServiceModelAsync(order,
                    db,
                    getAddressCityCountryFunc,
                    getOrderDetailsFunc);

                allOrders.Orders.Add(orderListingModel);

                var cart = await db.Carts.FindAsync(order!.CartId);

                await this.UpdateAllOrdersModel(db, cart!, allOrders);
            }

            allOrders.TotalOrders = allOrders.Orders.Count;
            allOrders = await this.FilterAllOrdersModel(allOrders, search, page, filters, startDate, endDate);

            return allOrders;
        }

        public async Task<List<OrderListingServiceModel>> AllForExport(string? search, string? filters, DateTime? startDate, DateTime? endDate)
        {
            var allOrders = new List<OrderListingServiceModel>();

            int currentPage = 1;
            while (true)
            {
                var data = await this.All(currentPage, search, filters, startDate, endDate);

                if (!data.Orders.Any())
                {
                    return allOrders;
                }

                foreach (var order in data.Orders)
                {
                    allOrders.Add(order);
                }

                currentPage++;
            }
        }

        public async Task<AllOrdersServiceModel> Mine(int page, string? search)
        {
            var userOrders = db.UsersOrders
                .Where(x => x.ProfileId == currentUserService.GetUserId())
                .OrderByDescending(x => x.CreatedOn)
                .AsQueryable();

            var allOrders = new AllOrdersServiceModel()
            {
                TotalOrders = await userOrders.CountAsync()
            };

            foreach (var userOrder in userOrders)
            {
                var order = await this.GetOrder(db, userOrder.OrderId);
                var orderModel = await OrderFactory.CreateOrderListingServiceModelAsync(order!,
                    db,
                    getAddressCityCountryFunc,
                    getOrderDetailsFunc);

                allOrders.Orders.Add(orderModel);

                var cart = await db.Carts.FindAsync(order!.CartId);

                await this.UpdateAllOrdersModel(db, cart!, allOrders);
            }

            allOrders = await this.FilterAllOrdersModel(allOrders, search, page, "", null, null);

            return allOrders;
        }

        public async Task<OrderServiceModel?> GetFinishedOrder(int orderId, string? token)
        {
            var orderFromDb = await this.GetOrder(db, orderId);

            if (orderFromDb == null)
                return null;

            var (customerName, customerEmail, phoneNumber) = await this.GetCurrentOrderUserDetailsByUser(db,
                orderFromDb,
                currentUserService,
                token);

            var cart = await db.Carts
                .FirstAsync(x => x.Id == orderFromDb.CartId);

            var cartModel = await CartFactory.CreateCartServiceModelAsync(cart,
                db,
                customerName,
                customerEmail,
                phoneNumber);

            var orderDetails = await this.GetOrderDetails(db, orderFromDb.OrderDetailsId);
            var (address, city, country) = await this.GetAddressCityCountry(db, orderDetails);

            var order = await OrderFactory.CreateOrderServiceModelAsync(db,
                mapper,
                cartModel,
                orderFromDb,
                orderDetails,
                city,
                country,
                address,
                customerEmail,
                customerName,
                config.GetSection("IBAN").Value,
                phoneNumber);

            return order;
        }

        public async Task<OrderServiceModel?> GetFinishedByAdmin(int orderId)
        {
            var orderFromDb = await this.GetOrder(db, orderId);

            if (orderFromDb == null)
                return null;

            var (customerName, customerEmail, phoneNumber) = await this.GetCurrentOrderUserDetailsByAdmin(db, orderFromDb);

            var cart = await db.Carts
                .FirstAsync(x => x.Id == orderFromDb.CartId);

            var cartModel = await CartFactory.CreateCartServiceModelAsync(cart,
                db,
                customerName,
                customerEmail,
                phoneNumber);

            var orderDetails = await this.GetOrderDetails(db, orderFromDb.OrderDetailsId);
            var (address, city, country) = await this.GetAddressCityCountry(db, orderDetails);

            var order = await OrderFactory.CreateOrderServiceModelAsync(db,
                mapper,
                cartModel,
                orderFromDb,
                orderDetails,
                city,
                country,
                address,
                customerEmail,
                customerName,
                config.GetSection("IBAN").Value,
                phoneNumber);

            return order;
        }

        public async Task<bool> ConfirmOrder(int orderId)
        {
            var order = await this.GetOrder(db, orderId);

            if (order == null)
                return false;

            if (!order.IsConfirmed)
            {
                var cart = await db.Carts
                    .FirstAsync(x => x.Id == order.CartId);

                var cartProducts = db.CartProducts
                    .Where(x => x.CartId == cart.Id);

                var lowStocks = new List<LowInStockServiceModel>();

                foreach (var cartProduct in cartProducts)
                {
                    var product = await db.Products
                        .FirstAsync(x => x.ProductId == cartProduct.ProductId);

                    var productPackageFlavour = await db.ProductsPackagesFlavours
                        .FirstAsync(x => x.ProductId == cartProduct.ProductId &&
                        x.FlavourId == cartProduct.FlavourId &&
                        x.PackageId == cartProduct.PackageId);

                    product.Quantity -= cartProduct.Count;
                    productPackageFlavour.Quantity -= cartProduct.Count;

                    this.CheckForLowStock(product, lowStocks);
                }

                await this.SendLowStockNotifications(lowStocks, notificationService, orderId);

                await notificationService.SendNotificationToAdmin("success", string.Format(OrderHasJustBeenConfirmed, order.Id));

                order.IsConfirmed = true;

                await db.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task SetShippingPrice(int cartId, string countryName)
        {
            var cart = await db.Carts
                .FirstAsync(x => x.Id == cartId);

            var country = await db.Countries
                .FirstAsync(x => x.CountryName == countryName);

            if (country.ShippingDiscountId != null)
            {
                var shippingDiscount = await db.ShippingDiscounts
                    .FirstAsync(x => x.Id == country.ShippingDiscountId);

                if ((shippingDiscount.MinimumPrice != null &&
                    cart.TotalProducts >= shippingDiscount.MinimumPrice) ||
                    shippingDiscount.MinimumPrice == null
                    )
                {
                    var priceWithDiscount = country.ShippingPrice * ((100 - shippingDiscount.DiscountPercentage) / 100);
                    cart.ShippingPrice = priceWithDiscount;
                    cart.TotalSaved = country.ShippingPrice - priceWithDiscount;
                }
                else
                {
                    cart.ShippingPrice = country.ShippingPrice;
                }
            }
            else
            {
                cart.ShippingPrice = country.ShippingPrice;
            }
        }

        public async Task<OrderRelatedProductsServiceModel> GetOrderRelatedProducts(decimal price)
        {
            var productsPackagesFlavours = db.ProductsPackagesFlavours
                .Where(x => x.Quantity > 0)
                .AsQueryable();

            var relatedProducts = new OrderRelatedProductsServiceModel();

            foreach (var productPackageFlavour in productsPackagesFlavours)
            {
                if (productPackageFlavour.Price >= price)
                {
                    var productModel = await ProductFactory.CreateOrderRelatedProductModelAsync(db, productPackageFlavour);
                    relatedProducts.Products.Add(productModel);
                }
            }

            relatedProducts.Products = relatedProducts.Products
                .OrderBy(x => x.Price)
                .Take(4)
                .ToList();

            return relatedProducts;
        }

        public async Task<bool> ChangeStatuses(int orderId, bool isFinished, bool isPaid, bool isShipped, bool isConfirmed)
        {
            var order = await this.GetOrder(db, orderId);

            if (order == null)
                return false;

            var details = await this.GetOrderDetails(db, order.OrderDetailsId);

            order.IsFinished = isFinished;
            details.IsPaid = isPaid;
            details.IsShipped = isShipped;

            if (isConfirmed && !order.IsConfirmed)
            {
                await ConfirmOrder(order.Id);
            }
            else
            {
                await db.SaveChangesAsync();
            }

            return true;
        }

        public async Task DeleteById(int orderId)
        {
            var order = await this.GetOrder(db, orderId);

            if (order == null)
                throw new ArgumentNullException(OrderCouldNotBeDeleted);

            if (order.IsFinished == false)
                throw new InvalidOperationException(OrderMustBeFinishedBeforeDeletingIt);

            var orderDetails = await this.GetOrderDetails(db, order.OrderDetailsId);

            order.IsDeleted = true;
            order.DeletedOn = DateTime.UtcNow;
            order.DeletedBy = currentUserService.GetUserName() ?? "";
            orderDetails.IsDeleted = true;

            if (orderDetails.InvoiceId != null)
            {
                var invoice = await db.Invoices
                    .FirstAsync(x => x.Id == orderDetails.InvoiceId);

                invoice.IsDeleted = true;
            }

            if (order.GuestOrderId != null)
            {
                var guestOrder = await db.GuestsOrders
                    .FirstAsync(x => x.Id == order.GuestOrderId);

                guestOrder.IsDeleted = true;
            }

            if (order.UserOrderId != null)
            {
                var userOrder = await db.UsersOrders
                    .FirstAsync(x => x.Id == order.UserOrderId);

                userOrder.IsDeleted = true;
            }

            await db.SaveChangesAsync();

            return;
        }

        public async Task<OrdersSummaryServiceModel> Summary()
        {
            var summary = new OrdersSummaryServiceModel();

            var orders = db.Orders
                .AsQueryable();

            summary.TotalOrders = await orders.CountAsync();
            foreach (var order in orders)
            {
                var cart = await db.Carts.FirstAsync(x => x.Id == order.CartId);

                await Task.Run(() =>
                {
                    summary.TotalDiscounts += cart!.TotalSaved; // be aware
                    summary.TotalPriceWithoutDiscount += cart.TotalSaved + cart.TotalProducts + (cart.ShippingPrice ?? 0);
                    summary.TotalProducts += db.CartProducts
                                                    .Where(x => x.CartId == cart.Id)
                                                    .Select(x => x.Count)
                                                    .Sum();
                    summary.TotalPrice += cart.TotalProducts + (cart.ShippingPrice ?? 0);
                });
            }

            return summary;
        }
    }
}
