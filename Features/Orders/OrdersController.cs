namespace NutriBest.Server.Features.Orders
{
    using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult<OrderServiceModel>> All([FromQuery] string? search,
            [FromQuery] int page = 1)
        {
            try
            {
                var allOrders = await orderService.All(page, search);

                return Ok(allOrders);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        //[HttpGet]
        //[Authorize(Roles = "User")]
        //[Route("Mine")]
        //public async Task<ActionResult<OrderServiceModel>> Mine([FromQuery] int page, [FromQuery] string? search)
        //{
        //    try
        //    {
        //        var allOrders = await orderService.Mine();

        //        return Ok(allOrders);
        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest();
        //    }
        //}

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

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("Admin/{orderId}")]
        public async Task<ActionResult<OrderServiceModel>> GetByIdFromAdmin([FromRoute] int orderId)
        {
            try
            {
                var order = await orderService.GetFinishedByAdmin(orderId);

                return Ok(order);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("change-status/{orderId}")]
        public async Task<ActionResult<bool>> ChangeStatuses([FromBody] UpdateOrderServiceModel orderModel,
            [FromRoute] int orderId)
        {
            try
            {
                var result = await orderService.ChangeStatuses(orderId,
                    orderModel.IsFinished,
                    orderModel.IsPaid,
                    orderModel.IsShipped);

                return Ok(new
                {
                    Successful = true
                });
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    Successful = false
                });
            }
        }

        [HttpPost]
        [Route("Confirm")]
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

        [HttpDelete]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("Admin/{orderId}")]
        public async Task<ActionResult> Delete([FromRoute] int orderId)
        {
            try
            {
                await orderService.DeleteById(orderId);

                return Ok();
            }
            catch (ArgumentNullException err)
            {
                return BadRequest(new
                {
                    err.Message
                });
            }
            catch (InvalidOperationException err)
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

        private string? GetSessionOrder()
        {
            string cookieValue = Request.Cookies[OrderCookie]!; // be aware

            if (string.IsNullOrEmpty(cookieValue))
                return null;

            return cookieValue;
        }
    }
}
