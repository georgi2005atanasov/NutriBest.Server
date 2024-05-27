namespace NutriBest.Server.Features.Orders
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using NutriBest.Server.Features.Carts.Models;
    using NutriBest.Server.Features.GuestOrders.Models;

    public class GuestsOrdersController : ApiController
    {
        private const string CartCookieName = "ShoppingCart";

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<int>> Create(GuestOrderServiceModel orderModel)
        {
            try
            {
                var cart = GetSessionCart();

                return Ok(orderModel);
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
