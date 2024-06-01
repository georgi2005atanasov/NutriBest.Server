namespace NutriBest.Server.Features.Orders
{
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.Orders.Models;

    public class OrdersController : ApiController
    {
        private const string OrderCookie = "OrderCookie";
        private readonly IOrderService orderService;

        public OrdersController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpGet]
        [Route("{orderId}")]
        public async Task<ActionResult<OrderServiceModel>> GetById([FromRoute] int orderId)
        {
            try
            {
                var token = GetSessionOrder();

                var order = await orderService.GetFinishedOrder(orderId, token);

                return Ok(order);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("confirm")]
        public async Task<ActionResult<bool>> ConfirmOrder([FromQuery] int orderId)
        {
            try
            {
                var result = await orderService.ConfirmOrder(orderId);

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        private string? GetSessionOrder()
        {
            string cookieValue = Request.Cookies[OrderCookie]!; // be aware

            if (string.IsNullOrEmpty(cookieValue))
                return null;

            return cookieValue;
        }
    }
}
