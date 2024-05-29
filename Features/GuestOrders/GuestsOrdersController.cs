namespace NutriBest.Server.Features.Orders
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Enums;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Carts.Models;
    using NutriBest.Server.Features.GuestOrders.Models;
    using NutriBest.Server.Features.OrderDetails;

    public class GuestsOrdersController : ApiController
    {
        private const string CartCookieName = "ShoppingCart";
        private readonly NutriBestDbContext db;
        private readonly IGuestOrderService guestOrderService;
        private readonly IOrderDetailsService orderDetailsService;

        public GuestsOrdersController(NutriBestDbContext db,
            IGuestOrderService guestOrderService,
            IOrderDetailsService orderDetailsService)
        {
            this.db = db;
            this.guestOrderService = guestOrderService;
            this.orderDetailsService = orderDetailsService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<int>> Create([FromBody] GuestOrderServiceModel orderModel)
        {
            if (orderModel.HasInvoice &&
                (orderModel.Invoice == null ||
                orderModel.Invoice.CompanyName == null ||
                orderModel.Invoice.FirstName == null ||
                orderModel.Invoice.LastName == null ||
                orderModel.Invoice.PhoneNumber == null ||
                orderModel.Invoice.PersonInCharge == null))
            {
                return BadRequest(new
                {
                    Message = "Fill the invoice form!"
                });
            }

            try
            {
                PaymentMethod paymentMethod;

                Enum.TryParse<PaymentMethod>(orderModel.PaymentMethod, true, out paymentMethod);

                if (paymentMethod.ToString() != orderModel.PaymentMethod)
                    return BadRequest(new
                    {
                        Message = "Invalid payment method!"
                    });

                var cookieCart = GetSessionCart() ?? new CartServiceModel();

                if (cookieCart.OriginalPrice == 0)
                {
                    return BadRequest(new
                    {
                        Message = "You have to purchase something!"
                    });
                }

                var cartId = await guestOrderService.PrepareCart(cookieCart.TotalPrice,
                    cookieCart.OriginalPrice,
                    cookieCart.TotalSaved,
                    cookieCart.Code,
                    cookieCart.CartProducts);

                var order = new Order
                {
                    CartId = cartId,
                    IsFinished = false,
                    IsConfirmed = false
                };
                
                order.OrderDetailsId = await orderDetailsService.CreateAnonymous(orderModel.CountryName,
                    orderModel.City,
                    orderModel.Street,
                    orderModel.StreetNumber,
                    orderModel.PostalCode,
                    orderModel.PaymentMethod,
                    orderModel.HasInvoice,
                    orderModel.Invoice,
                    orderModel.Comment);

                db.Orders.Add(order);

                await db.SaveChangesAsync();

                var guestOrderId = await guestOrderService.CreateGuestOrder(order.Id,
                    orderModel.Email,
                    orderModel.PaymentMethod,
                    orderModel.PhoneNumber);

                return Ok(new
                {
                    Id = guestOrderId
                });
            }
            catch (ArgumentNullException err)
            {
                return BadRequest(new
                {
                    err.Message
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        private CartServiceModel? GetSessionCart()
        {
            string cookieValue = Request.Cookies[CartCookieName]!; // be aware

            if (string.IsNullOrEmpty(cookieValue))
                return new CartServiceModel();

            var cart = JsonConvert.DeserializeObject<CartServiceModel>(cookieValue);
            return cart;
        }
    }
}
