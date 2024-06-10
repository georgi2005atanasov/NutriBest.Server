namespace NutriBest.Server.Features.Orders.Extensions
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;

    public static class OrderServiceExtensions
    {
        public static async Task<(string customerName, string customerEmail, string phoneNumber)> GetCurrentOrderUserDetails(this IOrderService service, NutriBestDbContext db, Order orderFromDb)
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

        public static async Task<(Address address, City city, Country country)> GetAddressCityCountry(this OrderService service, NutriBestDbContext db, OrderDetails orderDetails)
        {
            var address = await db.Addresses
                .FirstAsync(x => x.Id == orderDetails.AddressId);
            var city = await db.Cities
                .FirstAsync(x => x.Id == address.CityId);
            var country = await db.Countries
                .FirstAsync(x => x.Id == address.CountryId);

            return (address, city, country);
        }

        public static async Task<OrderDetails> GetOrderDetails(this IOrderService service, NutriBestDbContext db, int orderDetailsId)
            => await db.OrdersDetails
                .FirstAsync(x => x.Id == orderDetailsId);

        public static async Task<Order?> GetOrder(this IOrderService service, NutriBestDbContext db, int orderId)
            => await db.Orders
                .FirstOrDefaultAsync(x => x.Id == orderId);
    }
}
