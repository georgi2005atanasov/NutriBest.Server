namespace NutriBest.Server.Features.Orders
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.Orders.Models;
    using NutriBest.Server.Utilities;
    using System.Globalization;
    using System.Text;
    using System.Web;

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
        public async Task<ActionResult<AllOrdersServiceModel>> All([FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] string? filters = "",
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null)
        {
            try
            {
                var (parsedStartDate, parsedEndDate) = DateTimeHelper.ParseDates(startDate, endDate);

                var allOrders = await orderService.All(page, search, filters, parsedStartDate, parsedEndDate);

                return Ok(allOrders);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        [Route("Mine")]
        public async Task<ActionResult<AllOrdersServiceModel>> Mine([FromQuery] int page, [FromQuery] string? search)
        {
            try
            {
                var currentUserOrders = await orderService.Mine(page, search);

                return Ok(currentUserOrders);
            }
            catch (Exception)
            {
                return BadRequest();
            }
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

        [HttpGet]
        [Route("RelatedProducts")]
        public async Task<ActionResult<OrderRelatedProductsServiceModel>> GetRelatedProducts([FromQuery] decimal price)
        {
            try
            {
                var relatedProducts = await orderService.GetOrderRelatedProducts(price);

                return Ok(relatedProducts);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("ChangeStatus/{orderId}")]
        public async Task<ActionResult<bool>> ChangeStatuses([FromBody] UpdateOrderServiceModel orderModel,
            [FromRoute] int orderId)
        {
            try
            {
                var result = await orderService.ChangeStatuses(orderId,
                    orderModel.IsFinished,
                    orderModel.IsPaid,
                    orderModel.IsShipped,
                    orderModel.IsConfirmed);

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

                return Ok(new
                {
                    HasUpdated = result
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

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("CSV")]
        public async Task<FileContentResult?> GetCsv([FromQuery] string? search,
             [FromQuery] string? filters = "",
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null)
        {
            try
            {
                var (parsedStartDate, parsedEndDate) = DateTimeHelper.ParseDates(startDate, endDate);

                var products = await orderService.AllForExport(search, filters, parsedStartDate, parsedEndDate);
                var csv = ConvertToCsv(products);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "orders.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string ConvertToCsv(List<OrderListingServiceModel> orders)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Id,IsFinished,IsConfirmed,MadeOn,CustomerName,City,Country,Email,PhoneNumber,IsPaid,IsShipped,PaymentMethod,IsAnonymous,TotalPrice");

            if (orders == null)
            {
                return csv.ToString();
            }

            foreach (var order in orders)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(order.OrderId.ToString())},{CsvHelper.EscapeCsvValue(order.IsFinished.ToString())},{CsvHelper.EscapeCsvValue(order.IsConfirmed.ToString())},{CsvHelper.EscapeCsvValue(order.MadeOn.ToString())},{CsvHelper.EscapeCsvValue(order.CustomerName)},{CsvHelper.EscapeCsvValue(order.City)},{CsvHelper.EscapeCsvValue(order.Country)},{CsvHelper.EscapeCsvValue(order.Email)},{CsvHelper.EscapeCsvValue(order.PhoneNumber ?? "-")},{CsvHelper.EscapeCsvValue(order.IsPaid.ToString())},{CsvHelper.EscapeCsvValue(order.IsShipped.ToString())},{CsvHelper.EscapeCsvValue(order.PaymentMethod)},{CsvHelper.EscapeCsvValue(order.IsAnonymous.ToString())},{CsvHelper.EscapeCsvValue(order.TotalPrice.ToString())}");
            }

            return csv.ToString();
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("CSV/Summary")]
        public async Task<FileContentResult?> GetCsvSummary()
        {
            try
            {
                var summary = await orderService.Summary();
                var csv = ConvertToCsvSummary(summary);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "orders.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string ConvertToCsvSummary(SummaryServiceModel summary)
        {
            var csv = new StringBuilder();
            csv.AppendLine("TotalProducts,TotalOrders,TotalDiscounts,TotalPriceWithoutDiscount,TotalPrice");

            if (summary == null)
            {
                return csv.ToString();
            }
            csv.AppendLine($"{CsvHelper.EscapeCsvValue($"{summary.TotalProducts}")},{CsvHelper.EscapeCsvValue($"{summary.TotalOrders}")},{CsvHelper.EscapeCsvValue($"{summary.TotalDiscounts} BGN")},{CsvHelper.EscapeCsvValue($"{summary.TotalPriceWithoutDiscount} BGN")},{CsvHelper.EscapeCsvValue($"{summary.TotalPrice} BGN")}");

            return csv.ToString();
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
