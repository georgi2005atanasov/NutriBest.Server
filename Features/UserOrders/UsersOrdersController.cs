namespace NutriBest.Server.Features.UserOrders
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Enums;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Carts.Models;
    using NutriBest.Server.Features.OrderDetails;
    using NutriBest.Server.Features.UserOrders.Models;
    using NutriBest.Server.Infrastructure.Services;

    public class UsersOrdersController : ApiController
    {
        private const string CartCookieName = "ShoppingCart";
        private readonly NutriBestDbContext db;
        private readonly IUserOrderService userOrderService;
        private readonly IOrderDetailsService orderDetailsService;
        private readonly ICurrentUserService currentUserService;

        public UsersOrdersController(NutriBestDbContext db,
            IUserOrderService userOrderService,
            IOrderDetailsService orderDetailsService,
            ICurrentUserService currentUserService)
        {
            this.db = db;
            this.userOrderService = userOrderService;
            this.orderDetailsService = orderDetailsService;
            this.currentUserService = currentUserService;
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<int>> Create([FromBody] UserOrderServiceModel orderModel)
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

            if (!int.TryParse(orderModel.PostalCode, out var postalCode))
                return BadRequest(new
                {
                    Message = "Invalid postal code!"
                });

            try
            {

                var cookieCart = GetSessionCart() ?? new CartServiceModel();

                if (cookieCart.OriginalPrice == 0)
                {
                    return BadRequest(new
                    {
                        Message = "You have to purchase something!"
                    });
                }

                var cartId = await userOrderService.PrepareCart(cookieCart.TotalPrice,
                    cookieCart.OriginalPrice,
                    cookieCart.TotalSaved,
                    cookieCart.Code,
                    cookieCart.CartProducts);

                var order = new Order
                {
                    CartId = cartId,
                    IsFinished = false,
                    IsConfirmed = false,
                    Comment = orderModel.Comment
                };

                var userId = currentUserService.GetUserId();

                if (userId == null)
                    return BadRequest(new
                    {
                        Message = "Invalid user!"
                    });

                order.OrderDetailsId = await orderDetailsService.Create(orderModel.Country,
                    orderModel.City,
                    orderModel.Street,
                    orderModel.StreetNumber,
                    postalCode,
                    orderModel.PaymentMethod,
                    orderModel.HasInvoice,
                    orderModel.Invoice,
                    orderModel.Comment,
                    userId); // mandatory

                db.Orders.Add(order);

                await db.SaveChangesAsync();

                var userOrderId = await userOrderService.CreateUserOrder(userId, // mandatory
                    order.Id,
                    orderModel.Email,
                    orderModel.PaymentMethod,
                    orderModel.PhoneNumber);

                return Ok(new
                {
                    Id = userOrderId
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
