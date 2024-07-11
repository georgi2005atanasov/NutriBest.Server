namespace NutriBest.Server.Features.Reports
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using NutriBest.Server.Utilities;
    using NutriBest.Server.Features.Reports.Models;

    [Authorize(Roles = "Administrator,Employee")]
    public class ReportsController : ApiController
    {
        private readonly IReportService reportService;

        public ReportsController(IReportService reportService)
            => this.reportService = reportService;

        [HttpGet]
        [Route(nameof(PerformanceInfo))]
        public async Task<ActionResult<PerformanceInfo>> PerformanceInfo([FromQuery] string? startDate,
            [FromQuery] string? endDate)
        {
            try
            {
                var (parsedStartDate, parsedEndDate) = DateTimeHelper.ParseDates(startDate, endDate);

                var info = await reportService.GetPerformanceInfo(parsedStartDate, parsedEndDate);

                return Ok(info);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route(nameof(DemographicsInfo))]
        public async Task<ActionResult<List<SellingCityServiceModel>>> DemographicsInfo([FromQuery] string? startDate,
            [FromQuery] string? endDate)
        {
            try
            {
                var (parsedStartDate, parsedEndDate) = DateTimeHelper.ParseDates(startDate, endDate);

                var cities = await reportService.GetTopCities(parsedStartDate, parsedEndDate);

                return Ok(cities);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
