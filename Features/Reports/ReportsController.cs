namespace NutriBest.Server.Features.Reports
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.Reports.Models;

    [Authorize(Roles = "Administrator")]
    public class ReportsController : ApiController
    {
        private readonly IReportService reportService;

        public ReportsController(IReportService reportService)
            => this.reportService = reportService;

        [HttpGet]
        [Route(nameof(PerformanceInfo))]
        public async Task<ActionResult<PerformanceInfo>> PerformanceInfo()
        {
            try
            {
                var info = await reportService.GetPerformanceInfo();

                return Ok(info);
            }
            catch (Exception err)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route(nameof(DemographicsInfo))]
        public async Task<ActionResult<List<SellingCityServiceModel>>> DemographicsInfo()
        {
            try
            {
                var cities = await reportService.GetTopCities();

                return Ok(cities);
            }
            catch (Exception err)
            {
                return NotFound();
            }
        }
    }
}
