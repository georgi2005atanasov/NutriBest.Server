﻿namespace NutriBest.Server.Features.Orders
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
    using NutriBest.Server.Features.Orders.Extensions;
    using NutriBest.Server.Features.PromoCodes;

    public class GuestsOrdersController : ApiController
    {
        private const string CartCookieName = "ShoppingCart";
        private const string OrderCookie = "OrderCookie";
        private readonly NutriBestDbContext db;
        private readonly IGuestOrderService guestOrderService;
        private readonly IOrderDetailsService orderDetailsService;
        private readonly IPromoCodeService promoCodeService;

        public GuestsOrdersController(NutriBestDbContext db,
            IGuestOrderService guestOrderService,
            IOrderDetailsService orderDetailsService,
            IPromoCodeService promoCodeService)
        {
            this.db = db;
            this.guestOrderService = guestOrderService;
            this.orderDetailsService = orderDetailsService;
            this.promoCodeService = promoCodeService;
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

            if (!Enum.TryParse<PaymentMethod>(orderModel.PaymentMethod, out var paymentMethod))
                return BadRequest(new
                {
                    Message = "Invalid postal code!"
                });


            int postalCode = 0;
            if (!string.IsNullOrEmpty(orderModel.PostalCode) && (!int.TryParse(orderModel.PostalCode, out postalCode)))
                return BadRequest(new
                {
                    Message = "Invalid postal code!"
                });

            if (await db.Users.AnyAsync(x => x.Email == orderModel.Email))
                return BadRequest(new
                {
                    Message = "User with this email already exists!"
                });

            var cookieCart = GetSessionCart() ?? new CartServiceModel();

            if (cookieCart.OriginalPrice == 0 || !cookieCart.CartProducts.Any())
            {
                return BadRequest(new
                {
                    Message = "You have to purchase something!"
                });
            }

            try
            {
                var cartId = await guestOrderService.PrepareCart(cookieCart.TotalPrice,
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

                db.Orders.Add(order);

                await db.SaveChangesAsync();

                var guestOrderId = await guestOrderService.CreateGuestOrder(order.Id,
                    orderModel.Name,
                    orderModel.Email,
                    orderModel.PhoneNumber);

                order.GuestOrderId = guestOrderId;

                if (!string.IsNullOrEmpty(cookieCart.Code))
                    await promoCodeService.DisableByCode(cookieCart.Code);

                await SetSessionOrder(token);
                await SetSessionCart(new CartServiceModel());

                await db.SaveChangesAsync();

                return Ok(new
                {
                    Id = order.Id
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
