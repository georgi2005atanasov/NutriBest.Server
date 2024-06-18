﻿namespace NutriBest.Server.Features.Orders.Extensions
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Notifications;
    using NutriBest.Server.Features.Orders.Models;
    using NutriBest.Server.Infrastructure.Services;
    using static ServicesConstants.PaginationConstants;
    using static ServicesConstants.Product;

    public static class OrderServiceExtensions
    {
        public static async Task<(string customerName, string customerEmail, string phoneNumber)>
            GetCurrentOrderUserDetailsByAdmin(this IOrderService service,
            NutriBestDbContext db,
            Order orderFromDb)
        {
            string customerName = "";
            string customerEmail = "";
            string phoneNumber = "";

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
                phoneNumber = user.PhoneNumber;
            }

            if (orderFromDb.GuestOrderId != null)
            {
                var guestOrder = await db.GuestsOrders
                    .FirstAsync(x => x.Id == orderFromDb.GuestOrderId);

                customerName = guestOrder.Name;
                customerEmail = guestOrder.Email;
                phoneNumber = guestOrder.PhoneNumber ?? "";
            }

            return (customerName, customerEmail, phoneNumber);
        }

        public static async Task<(string customerName, string customerEmail, string phoneNumber)>
            GetCurrentOrderUserDetailsByUser(this IOrderService service,
            NutriBestDbContext db,
            Order orderFromDb,
            ICurrentUserService currentUserService,
            string? token)
        {
            string customerName = "";
            string customerEmail = "";
            string phoneNumber = "";

            if (orderFromDb.UserOrderId != null)
            {
                var userOrder = await db.UsersOrders.FirstAsync(x => x.OrderId == orderFromDb.Id);
                var profile = await db.Profiles.FirstAsync(x => x.UserId == userOrder.ProfileId);
                customerName = profile.Name!;

                var user = await db.Users.FirstAsync(x => x.Id == userOrder.ProfileId);
                customerEmail = user.Email;
                phoneNumber = user.PhoneNumber ?? "";

                if (userOrder.ProfileId != currentUserService.GetUserId())
                {
                    throw new InvalidOperationException();
                }
            }
            else if (orderFromDb.GuestOrderId != null)
            {
                var guestOrder = await db.GuestsOrders.FirstAsync(x => x.Id == orderFromDb.GuestOrderId);
                customerName = guestOrder.Name;
                customerEmail = guestOrder.Email;
                phoneNumber = guestOrder.PhoneNumber ?? "";

                if (orderFromDb.SessionToken != token)
                    throw new InvalidOperationException();

                if (currentUserService.GetUserId() != null)
                    throw new InvalidOperationException();
            }

            return (customerName, customerEmail, phoneNumber);
        }

        public static async Task<(Address address, City city, Country country)> GetAddressCityCountry(this OrderService service,
            NutriBestDbContext db,
            OrderDetails orderDetails)
        {
            var address = await db.Addresses
                .FirstAsync(x => x.Id == orderDetails.AddressId);
            var city = await db.Cities
                .FirstAsync(x => x.Id == address.CityId);
            var country = await db.Countries
                .FirstAsync(x => x.Id == address.CountryId);

            return (address, city, country);
        }

        public static async Task UpdateAllOrdersModel(this IOrderService service,
            NutriBestDbContext db,
            Cart cart,
            AllOrdersServiceModel allOrders)
        {
            await Task.Run(() =>
            {
                allOrders.TotalDiscounts += cart!.TotalSaved; // be aware
                allOrders.TotalPriceWithoutDiscount += cart.TotalSaved + cart.TotalProducts + (cart.ShippingPrice ?? 0);
                allOrders.TotalProducts += db.CartProducts
                                                .Where(x => x.CartId == cart.Id)
                                                .Select(x => x.Count)
                                                .Sum();
                allOrders.TotalPrice += cart.TotalProducts + (cart.ShippingPrice ?? 0);
            });
        }

        public static AllOrdersServiceModel FilterAllOrdersModel(this IOrderService service,
            AllOrdersServiceModel allOrders,
            string? search,
            int page,
            string? filters)
        {
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                allOrders.Orders = allOrders.Orders
                    .Where(x => $"{x.OrderId}" == search ||
                    (x.PhoneNumber != null && x.PhoneNumber.ToLower().Contains(search)) ||
                    x.CustomerName.ToLower().Contains(search))
                    .OrderByDescending(x => x.MadeOn)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(filters))
            {
                List<OrderListingServiceModel> filteredOrders = new List<OrderListingServiceModel>();

                var allFilters = filters.Split(" ");

                if (allFilters.Contains("Finished"))
                {
                    filteredOrders.AddRange(allOrders.Orders.Where(x => x.IsFinished));
                }
                if (allFilters.Contains("Confirmed"))
                {
                    filteredOrders.AddRange(allOrders.Orders.Where(x => x.IsConfirmed));
                }
                if (allFilters.Contains("Paid"))
                {
                    filteredOrders.AddRange(allOrders.Orders.Where(x => x.IsPaid));
                }
                if (allFilters.Contains("Shipped"))
                {
                    filteredOrders.AddRange(allOrders.Orders.Where(x => x.IsAnonymous));
                }

                allOrders.Orders = filteredOrders;
            }

            allOrders.Orders = allOrders.Orders
                .Skip((page - 1) * OrdersPerPage)
                .Take(OrdersPerPage)
                .ToList();

            return allOrders;
        }

        public static void CheckForLowStock(this IOrderService service,
            Product product,
            List<LowInStockServiceModel> lowStocks)
        {
            if (product.Quantity < StockLowPriority)
            {
                lowStocks.Add(new LowInStockServiceModel
                {
                    Name = product.Name,
                    ProductId = product.ProductId,
                    Quantity = (int)product.Quantity!
                });
            }
        }

        public static async Task SendLowStockNotifications(this IOrderService service,
            List<LowInStockServiceModel> lowStocks,
            INotificationService notificationService,
            int orderId)
        {
            foreach (var product in lowStocks)
            {
                await notificationService.SendLowInStockNotification(product.Name, product.ProductId, product.Quantity, $"#000000{orderId}");
            }
        }

        public static async Task<OrderDetails> GetOrderDetails(this IOrderService service,
            NutriBestDbContext db, int
            orderDetailsId)
            => await db.OrdersDetails
                .FirstAsync(x => x.Id == orderDetailsId);

        public static async Task<Order?> GetOrder(this IOrderService service,
            NutriBestDbContext db,
            int orderId)
            => await db.Orders
                .FirstOrDefaultAsync(x => x.Id == orderId);
    }
}
