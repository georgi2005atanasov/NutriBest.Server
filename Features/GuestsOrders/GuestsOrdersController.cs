using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Orders
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Enums;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Shared.Responses;
    using NutriBest.Server.Features.Carts.Models;
    using NutriBest.Server.Features.GuestsOrders.Models;
    using NutriBest.Server.Features.Notifications;
    using NutriBest.Server.Features.OrderDetails;
    using NutriBest.Server.Features.Orders.Extensions;
    using NutriBest.Server.Features.PromoCodes;
    using static ErrorMessages.GuestsOrdersController;
    using static SuccessMessages.NotificationService;

    public class GuestsOrdersController : ApiController
    {
        private const string CartCookieName = "ShoppingCart";
        private const string OrderCookie = "OrderCookie";
        private readonly NutriBestDbContext db;
        private readonly IGuestOrderService guestOrderService;
        private readonly IOrderDetailsService orderDetailsService;
        private readonly IPromoCodeService promoCodeService;
        private readonly INotificationService notificationService;

        public GuestsOrdersController(NutriBestDbContext db,
            IGuestOrderService guestOrderService,
            IOrderDetailsService orderDetailsService,
            IPromoCodeService promoCodeService,
            INotificationService notificationService)
        {
            this.db = db;
            this.guestOrderService = guestOrderService;
            this.orderDetailsService = orderDetailsService;
            this.promoCodeService = promoCodeService;
            this.notificationService = notificationService;
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
                return BadRequest(new FailResponse
                {
                    Message = FillInvoiceForm
                });
            }

            if (!Enum.TryParse<PaymentMethod>(orderModel.PaymentMethod, out var paymentMethod))
                return BadRequest(new
                {
                    Message = InvalidPaymentMethod
                });

            int postalCode = 0;
            if (!string.IsNullOrEmpty(orderModel.PostalCode) && (!int.TryParse(orderModel.PostalCode, out postalCode)))
                return BadRequest(new
                {
                    Message = InvalidPostalCode
                });

            if (await db.Users.AnyAsync(x => x.Email == orderModel.Email))
                return BadRequest(new
                {
                    Message = UserWithThisEmailAlreadyExists
                });

            var cookieCart = GetSessionCart() ?? new CartServiceModel();

            if (cookieCart.OriginalPrice == 0 || !cookieCart.CartProducts.Any())
            {
                return BadRequest(new
                {
                    Message = PurchaseIsRequiredToHaveSomething
                });
            }

            try
            {
                var cartId = await guestOrderService.PrepareCart(cookieCart.TotalProducts,
                    cookieCart.OriginalPrice,
                    cookieCart.TotalSaved,
                    cookieCart.Code,
                    cookieCart.CartProducts);

                var token = TokenGenerator.GenerateToken();
                var order = new Order
                {
                    CartId = cartId,
                    IsFinished = false,
                    IsConfirmed = false,
                    Comment = orderModel.Comment,
                    SessionToken = token
                };

                order.OrderDetailsId = await orderDetailsService.Create(orderModel.Country,
                    orderModel.City,
                    orderModel.Street,
                    orderModel.StreetNumber,
                    postalCode != 0 ? postalCode : null,
                    orderModel.PaymentMethod,
                    orderModel.HasInvoice,
                    orderModel.Invoice,
                    orderModel.Comment);

                await guestOrderService.SetShippingPrice(cartId, orderModel.Country); // new

                db.Orders.Add(order);

                await db.SaveChangesAsync();

                var guestOrderId = await guestOrderService.CreateGuestOrder(order.Id,
                    orderModel.Name,
                    orderModel.Email,
                    orderModel.PhoneNumber);

                order.GuestOrderId = guestOrderId;

                if (!string.IsNullOrEmpty(cookieCart.Code))
                    await promoCodeService.DisableByCode(cookieCart.Code);

                decimal totalOrderPrice = cookieCart.TotalProducts;

                await SetSessionOrder(token);
                await SetSessionCart(new CartServiceModel());

                await notificationService.SendNotificationToAdmin("success", string.Format(UserHasJustMadeAnOrder, orderModel.Name, $"{totalOrderPrice:f2}"));

                await db.SaveChangesAsync();

                return Ok(new
                {
                    Id = order.Id
                });
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.Message
                });
            }
            catch (ArgumentNullException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.ParamName ?? ""
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

        private async Task SetSessionOrder(string sessionToken)
        {
            await Task.Run(() =>
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true, // this was commented to show the cookie as I type document.cookie
                    Expires = DateTime.UtcNow.AddDays(7),
                    Secure = true,
                    SameSite = SameSiteMode.None
                };

                Response.Cookies.Append(OrderCookie, sessionToken, cookieOptions);
            });
        }

        private async Task SetSessionCart(CartServiceModel cart)
        {
            await Task.Run(() =>
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true, // this was commented to show the cookie as I type document.cookie
                    Expires = DateTime.UtcNow.AddDays(7),
                    Secure = true,
                    SameSite = SameSiteMode.None
                };

                string serializedCart = JsonConvert.SerializeObject(cart);
                Response.Cookies.Append(CartCookieName, serializedCart, cookieOptions);
            });
        }
    }
}
