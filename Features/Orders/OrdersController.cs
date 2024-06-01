namespace NutriBest.Server.Features.Orders
{
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.Orders.Models;

    public class OrdersController : ApiController
    {
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
                var order = await orderService.GetFinishedOrder(orderId);

                return Ok(order);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
