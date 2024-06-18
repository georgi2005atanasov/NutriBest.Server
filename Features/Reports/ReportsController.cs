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
        [Route(nameof(TopSellingProducts))]
        public async Task<ActionResult<TopSellingProductsServiceModel>> TopSellingProducts()
        {
            try
            {
                var topProducts = await reportService.GetTopSellingProducts();

                return Ok(topProducts);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route(nameof(TopSellingBrands))]
        public async Task<ActionResult<TopSellingBrandsServiceModel>> TopSellingBrands()
        {
            try
            {
                var topBrands = await reportService.GetTopSellingBrands();

                return Ok(topBrands);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route(nameof(TopSellingFlavours))]
        public async Task<ActionResult<TopSellingProductsServiceModel>> TopSellingFlavours()
        {
            try
            {
                var topFlavours = await reportService.GetTopSellingFlavours();

                return Ok(topFlavours);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route(nameof(TopSellingCategories))]
        public async Task<ActionResult<TopSellingCategoriesServiceModel>> TopSellingCategories()
        {
            try
            {
                var topCategories = await reportService.GetTopSellingCategories();

                return Ok(topCategories);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
