namespace NutriBest.Server.Features.Orders
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Carts.Models;
    using NutriBest.Server.Features.Invoices.Models;
    using NutriBest.Server.Features.Orders.Models;
    using NutriBest.Server.Features.Products.Models;
    using NutriBest.Server.Infrastructure.Services;
    using static ServicesConstants.PaginationConstants;
    using NutriBest.Server.Features.Orders.Extensions;
    using NutriBest.Server.Features.Notifications;

    public class OrderService : IOrderService
    {
        protected readonly NutriBestDbContext db;
        private readonly ICurrentUserService currentUserService;
        private readonly IConfiguration config;
        private readonly INotificationService notificationService;

        public OrderService(NutriBestDbContext db,
            ICurrentUserService currentUserService,
            IConfiguration config,
            INotificationService notificationService)
        {
            this.db = db;
            this.currentUserService = currentUserService;
            this.config = config;
            this.notificationService = notificationService;
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

        public async Task<AllOrdersServiceModel> All(int page, string? search)
        {
            var orders = db.Orders
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.OrderDetails!.MadeOn)
                .AsQueryable();

            var allOrders = new AllOrdersServiceModel()
            {
                TotalOrders = await orders.CountAsync()
            };

            orders = orders
                .AsQueryable();

            foreach (var order in orders)
            {
                var orderDetails = await this.GetOrderDetails(db, order.OrderDetailsId);

                var (address, city, country) = await this.GetAddressCityCountry(db, orderDetails);

                var cart = await db.Carts
                    .FindAsync(order.CartId);

                if (order.GuestOrderId != null)
                {
                    var guestOrder = await db.GuestsOrders
                        .FindAsync(order.GuestOrderId);

                    var orderModel = new OrderListingServiceModel
                    {
                        CustomerName = guestOrder!.Name,
                        City = city != null ? city!.CityName : "",
                        Country = country != null ? country!.CountryName : "",
                        IsConfirmed = order.IsConfirmed,
                        IsFinished = order.IsFinished,
                        IsShipped = orderDetails.IsShipped,
                        IsPaid = orderDetails.IsPaid,
                        OrderId = order.Id,
                        MadeOn = orderDetails.MadeOn,
                        PaymentMethod = orderDetails.PaymentMethod.ToString(),
                        TotalPrice = cart!.TotalProducts + (cart.ShippingPrice ?? 0),
                        PhoneNumber = guestOrder.PhoneNumber,
                        Email = guestOrder.Email,
                        IsAnonymous = true
                    };

                    allOrders.Orders.Add(orderModel);
                }

                if (order.UserOrderId != null)
                {
                    var userOrder = await db.UsersOrders
                        .FirstAsync(x => x.Id == order.UserOrderId);

                    var profile = await db.Profiles
                        .FirstAsync(x => x.UserId == userOrder.ProfileId);

                    var user = await db.Users
                        .FirstAsync(x => x.Id == userOrder.ProfileId);

                    var orderModel = new OrderListingServiceModel
                    {
                        CustomerName = profile.Name ?? "",
                        City = city != null ? city!.CityName : "",
                        Country = country != null ? country!.CountryName : "",
                        IsConfirmed = order.IsConfirmed,
                        IsFinished = order.IsFinished,
                        IsShipped = orderDetails.IsShipped,
                        IsPaid = orderDetails.IsPaid,
                        OrderId = order.Id,
                        MadeOn = orderDetails.MadeOn,
                        PaymentMethod = orderDetails.PaymentMethod.ToString(),
                        TotalPrice = cart!.TotalProducts + (cart.ShippingPrice ?? 0),
                        PhoneNumber = user.PhoneNumber,
                        Email = user.Email,
                        IsAnonymous = false
                    };

                    allOrders.Orders.Add(orderModel);
                }

                allOrders.TotalDiscounts += cart!.TotalSaved; // be aware
                allOrders.TotalPriceWithoutDiscount += cart.TotalSaved + cart.TotalProducts + (cart.ShippingPrice ?? 0);
                allOrders.TotalProducts += await db.CartProducts
                                                .Where(x => x.CartId == cart.Id)
                                                .CountAsync();
                allOrders.TotalPrice += cart.TotalProducts + (cart.ShippingPrice ?? 0);
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                allOrders.Orders = allOrders.Orders
                    .Where(x => $"{x.OrderId}" == search ||
                    x.PhoneNumber.ToLower().Contains(search) ||
                    x.CustomerName.ToLower().Contains(search))
                    .OrderByDescending(x => x.MadeOn)
                    .ToList();
            }

            allOrders.Orders = allOrders.Orders
                .Skip((page - 1) * OrdersPerPage)
                .Take(OrdersPerPage)
                .ToList();

            return allOrders;
        }

        public async Task<OrderServiceModel?> GetFinishedOrder(int orderId, string? token)
        {
            var orderFromDb = await this.GetOrder(db, orderId);

            string customerName = "";
            string customerEmail = "";
            string phoneNumber = "";

            if (orderFromDb == null)
                return null;

            if (orderFromDb.UserOrderId != null)
            {
                var userOrder = await db.UsersOrders
                    .FirstAsync(x => x.OrderId == orderFromDb.Id);

                var profile = await db.Profiles
                    .FirstAsync(x => x.UserId == userOrder.ProfileId);

                customerName = profile.Name!;

                var user = await db.Users
                    .FirstAsync(x => x.Id == userOrder.ProfileId);

                customerEmail = user.Email;
                phoneNumber = user.PhoneNumber ?? "";

                if (userOrder.ProfileId != currentUserService.GetUserId())
                {
                    throw new InvalidOperationException();
                }
            }

            if (orderFromDb.GuestOrderId != null)
            {
                var guestOrder = await db.GuestsOrders
                    .FirstAsync(x => x.Id == orderFromDb.GuestOrderId);

                customerName = guestOrder.Name;
                customerEmail = guestOrder.Email;
                phoneNumber = guestOrder.PhoneNumber ?? "";

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

            await GetDiscountPercentageForTheProducts(cartProducts);

            var cartModel = new CartServiceModel
            {
                Code = cart.Code,
                OriginalPrice = cart.OriginalPrice,
                TotalProducts = cart.TotalProducts,
                TotalSaved = cart.TotalSaved,
                CartProducts = cartProducts
            };

            var orderDetails = await this.GetOrderDetails(db, orderFromDb.OrderDetailsId);
            var (address, city, country) = await this.GetAddressCityCountry(db, orderDetails);

            var order = new OrderServiceModel
            {
                Cart = cartModel,
                IsConfirmed = orderFromDb.IsConfirmed,
                IsFinished = orderFromDb.IsFinished,
                MadeOn = orderDetails.MadeOn,
                PaymentMethod = orderDetails.PaymentMethod.ToString(),
                IsPaid = orderDetails.IsPaid,
                IsShipped = orderDetails.IsShipped,
                Email = customerEmail,
                CustomerName = customerName,
                IBAN = config.GetSection("IBAN").Value,
                City = city.CityName,
                Country = country.CountryName,
                Street = address.Street,
                StreetNumber = address.StreetNumber,
                ShippingPrice = cart.ShippingPrice ?? 0,
                PhoneNumber = phoneNumber,
                Comment = orderFromDb.Comment
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

        public async Task<OrderServiceModel?> GetFinishedByAdmin(int orderId)
        {
            var orderFromDb = await this.GetOrder(db, orderId);

            if (orderFromDb == null)
                return null;

            var (customerName, customerEmail, phoneNumber) = await this.GetCurrentOrderUserDetails(db, orderFromDb);

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

            await GetDiscountPercentageForTheProducts(cartProducts);

            var cartModel = new CartServiceModel
            {
                Code = cart.Code,
                OriginalPrice = cart.OriginalPrice,
                TotalProducts = cart.TotalProducts,
                TotalSaved = cart.TotalSaved,
                CartProducts = cartProducts,
                ShippingPrice = cart.ShippingPrice
            };

            var orderDetails = await this.GetOrderDetails(db, orderFromDb.OrderDetailsId);
            var (address, city, country) = await this.GetAddressCityCountry(db, orderDetails);

            var order = new OrderServiceModel
            {
                Cart = cartModel,
                IsConfirmed = orderFromDb.IsConfirmed,
                IsFinished = orderFromDb.IsFinished,
                MadeOn = orderDetails.MadeOn,
                PaymentMethod = orderDetails.PaymentMethod.ToString(),
                IsPaid = orderDetails.IsPaid,
                IsShipped = orderDetails.IsShipped,
                Email = customerEmail,
                CustomerName = customerName,
                IBAN = config.GetSection("IBAN").Value,
                City = city.CityName,
                Country = country.CountryName,
                Street = address.Street,
                StreetNumber = address.StreetNumber,
                ShippingPrice = cart.ShippingPrice ?? 0,
                PhoneNumber = phoneNumber,
                Comment = orderFromDb.Comment
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

        public async Task<AllOrdersServiceModel> Mine(int page, string? search)
        {
            var userOrders = db.UsersOrders
                .Where(x => x.ProfileId == currentUserService.GetUserId())
                .AsQueryable();

            var allOrders = new AllOrdersServiceModel()
            {
                TotalOrders = await userOrders.CountAsync()
            };

            foreach (var userOrder in userOrders)
            {
                var order = await this.GetOrder(db, userOrder.OrderId);

                var orderDetails = await this.GetOrderDetails(db, order!.OrderDetailsId);

                var (address, city, country) = await this.GetAddressCityCountry(db, orderDetails);

                var cart = await db.Carts
                    .FindAsync(order.CartId);

                var profile = await db.Profiles
                    .FirstAsync(x => x.UserId == userOrder.ProfileId);

                var user = await db.Users
                    .FirstAsync(x => x.Id == userOrder.ProfileId);

                var orderModel = new OrderListingServiceModel
                {
                    CustomerName = profile.Name ?? "",
                    City = city != null ? city!.CityName : "",
                    Country = country != null ? country!.CountryName : "",
                    IsConfirmed = order.IsConfirmed,
                    IsFinished = order.IsFinished,
                    IsShipped = orderDetails.IsShipped,
                    IsPaid = orderDetails.IsPaid,
                    OrderId = order.Id,
                    MadeOn = orderDetails.MadeOn,
                    PaymentMethod = orderDetails.PaymentMethod.ToString(),
                    TotalPrice = cart!.TotalProducts + (cart.ShippingPrice ?? 0),
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    IsAnonymous = false
                };

                allOrders.Orders.Add(orderModel);


                allOrders.TotalDiscounts += cart!.TotalSaved; // be aware
                allOrders.TotalPriceWithoutDiscount += cart.TotalSaved + cart.TotalProducts + (cart.ShippingPrice ?? 0);
                allOrders.TotalProducts += await db.CartProducts
                                                .Where(x => x.CartId == cart.Id)
                                                .CountAsync();
                allOrders.TotalPrice += cart.TotalProducts + (cart.ShippingPrice ?? 0);
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                allOrders.Orders = allOrders.Orders
                    .Where(x => $"{x.OrderId}" == search ||
                    x.PhoneNumber.ToLower().Contains(search) ||
                    x.CustomerName.ToLower().Contains(search))
                    .OrderByDescending(x => x.MadeOn)
                    .ToList();
            }

            allOrders.Orders = allOrders.Orders
                .Skip((page - 1) * OrdersPerPage)
                .Take(OrdersPerPage)
                .ToList();

            return allOrders;
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

                    if (product.Quantity < 20)
                    {
                        lowStocks.Add(new LowInStockServiceModel
                        {
                            Name = product.Name,
                            ProductId = product.ProductId,
                            Quantity = (int)product.Quantity!
                        });
                    }
                }
                // reduce the quantity for each product in the cart

                order.IsConfirmed = true;

                await notificationService.SendNotificationToAdmin("success", $"Order #000000{order.Id} Has Just Been Confirmed!");

                await db.SaveChangesAsync();

                foreach (var product in lowStocks)
                {
                    await notificationService.SendLowInStockNotification(product.Name, product.ProductId, product.Quantity);
                }

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

                    relatedProducts.Products.Add(productModel);
                }
            }

            relatedProducts.Products = relatedProducts.Products
                .OrderBy(x => x.Price)
                .Take(4)
                .ToList();

            return relatedProducts;
        }

        private async Task GetDiscountPercentageForTheProducts(List<CartProductServiceModel> cartProducts)
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
                throw new ArgumentNullException("Order Could not be Deleted!");

            if (order.IsFinished == false)
                throw new InvalidOperationException("The order must be finished before deleting it!");

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
    }
}
