namespace NutriBest.Server.Features.Orders.Factories
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Carts.Models;
    using NutriBest.Server.Features.Invoices.Models;
    using NutriBest.Server.Features.Orders.Models;

    public static class OrderFactory
    {
        public static async Task<OrderListingServiceModel> CreateOrderListingServiceModelAsync(Order order, 
            NutriBestDbContext db,
            Func<NutriBestDbContext, OrderDetails, Task<(Address address, City city, Country country)>> getAddressCityCountry,
            Func<NutriBestDbContext, int, Task<OrderDetails>> getOrderDetails)
        {
            var orderDetails = await getOrderDetails.Invoke(db, order.OrderDetailsId);
            var (address, city, country) = await getAddressCityCountry.Invoke(db, orderDetails);
            var cart = await db.Carts.FindAsync(order.CartId);

            OrderListingServiceModel orderModel = new OrderListingServiceModel
            {
                City = city != null ? city.CityName : "",
                Country = country != null ? country.CountryName : "",
                IsConfirmed = order.IsConfirmed,
                IsFinished = order.IsFinished,
                IsShipped = orderDetails.IsShipped,
                IsPaid = orderDetails.IsPaid,
                OrderId = order.Id,
                MadeOn = orderDetails.MadeOn,
                PaymentMethod = orderDetails.PaymentMethod.ToString(),
                TotalPrice = cart!.TotalProducts + (cart.ShippingPrice ?? 0)
            };

            if (order.GuestOrderId != null)
            {
                var guestOrder = await db.GuestsOrders.FindAsync(order.GuestOrderId);
                orderModel.CustomerName = guestOrder!.Name;
                orderModel.PhoneNumber = guestOrder.PhoneNumber;
                orderModel.Email = guestOrder.Email;
                orderModel.IsAnonymous = true;
            }
            else if (order.UserOrderId != null)
            {
                var userOrder = await db.UsersOrders.FirstAsync(x => x.Id == order.UserOrderId);
                var profile = await db.Profiles.FirstAsync(x => x.UserId == userOrder.ProfileId);
                var user = await db.Users.FirstAsync(x => x.Id == userOrder.ProfileId);
                orderModel.CustomerName = profile.Name ?? "";
                orderModel.PhoneNumber = user.PhoneNumber;
                orderModel.Email = user.Email;
                orderModel.IsAnonymous = false;
            }

            return orderModel;
        }

        public static async Task<OrderServiceModel> CreateOrderServiceModelAsync(NutriBestDbContext db,
            IMapper mapper,
            CartServiceModel cartModel, 
            Order orderFromDb,
            OrderDetails orderDetails,
            City city,
            Country country,
            Address address,
            string customerEmail,
            string customerName,
            string IBAN,
            string phoneNumber)
        {
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
                IBAN = IBAN,
                City = city.CityName,
                Country = country.CountryName,
                Street = address.Street,
                StreetNumber = address.StreetNumber,
                ShippingPrice = cartModel.ShippingPrice ?? 0,
                PhoneNumber = phoneNumber,
                Comment = orderFromDb.Comment
            };

            if (orderDetails.InvoiceId != null)
            {
                var invoice = await db.Invoices
                    .FirstAsync(x => x.Id == orderDetails.InvoiceId);

                order.Invoice = mapper.Map<InvoiceServiceModel>(invoice);
            }

            return order;
        }
    }
}