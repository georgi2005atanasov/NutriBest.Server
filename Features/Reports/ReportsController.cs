namespace NutriBest.Server.Features.Reports
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.Reports.Models;
    using NutriBest.Server.Utilities;
    using System.Text;

    [Authorize(Roles = "Administrator")]
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

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("CSV/PerformanceInfo")]
        public async Task<FileContentResult?> GetCsvPerformanceInfo([FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null)
        {
            try
            {
                var (parsedStartDate, parsedEndDate) = DateTimeHelper.ParseDates(startDate, endDate);

                var products = await reportService.GetPerformanceInfo(parsedStartDate, parsedEndDate);
                var csv = ConvertToCsvPerformanceInfo(products);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "performanceInfo.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //some logic reuse may be a good thing
        private string ConvertToCsvPerformanceInfo(PerformanceInfo report)
        {
            var csv = new StringBuilder();

            csv.AppendLine(CsvHelper.EscapeCsvValue("Top Selling Products"));
            csv.AppendLine("Id,Name,StartingPrice,PromotionId,SoldCount");

            foreach (var data in report.TopSellingProducts)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(data.Product.ProductId.ToString())},{CsvHelper.EscapeCsvValue(data.Product.Name)},{CsvHelper.EscapeCsvValue($"{data.Product.Price} BGN")},{CsvHelper.EscapeCsvValue(data.Product.PromotionId?.ToString() ?? "-")},{data.SoldCount}");
            }

            csv.AppendLine();

            csv.AppendLine(CsvHelper.EscapeCsvValue("Weak Products"));
            csv.AppendLine("Id,Name,StartingPrice,PromotionId,SoldCount");

            foreach (var data in report.WeakProducts)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(data.Product.ProductId.ToString())},{CsvHelper.EscapeCsvValue(data.Product.Name)},{CsvHelper.EscapeCsvValue($"{data.Product.Price} BGN")},{CsvHelper.EscapeCsvValue(data.Product.PromotionId?.ToString() ?? "-")},{data.SoldCount}");
            }

            csv.AppendLine();

            csv.AppendLine(CsvHelper.EscapeCsvValue("Top Categories"));
            csv.AppendLine("Category,SoldCount");

            foreach (var data in report.TopSellingCategories)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(data.CategoryName)},{CsvHelper.EscapeCsvValue(data.SoldCount.ToString())}");
            }

            csv.AppendLine();

            csv.AppendLine(CsvHelper.EscapeCsvValue("Weak Categories"));
            csv.AppendLine("Category,SoldCount");

            foreach (var data in report.WeakCategories)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(data.CategoryName)},{CsvHelper.EscapeCsvValue(data.SoldCount.ToString())}");
            }

            csv.AppendLine();

            csv.AppendLine(CsvHelper.EscapeCsvValue("Top Flavours"));
            csv.AppendLine("Category,SoldCount");

            foreach (var data in report.TopSellingFlavours)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(data.FlavourName)},{CsvHelper.EscapeCsvValue(data.SoldCount.ToString())}");
            }

            csv.AppendLine();

            csv.AppendLine(CsvHelper.EscapeCsvValue("Weak Categories"));
            csv.AppendLine("Category,SoldCount");

            foreach (var data in report.WeakFlavours)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(data.FlavourName)},{CsvHelper.EscapeCsvValue(data.SoldCount.ToString())}");
            }

            csv.AppendLine();

            csv.AppendLine(CsvHelper.EscapeCsvValue("Top Brands"));
            csv.AppendLine("Category,SoldCount");

            foreach (var data in report.TopSellingBrands)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(data.BrandName)},{CsvHelper.EscapeCsvValue(data.SoldCount.ToString())}");
            }

            csv.AppendLine();

            csv.AppendLine(CsvHelper.EscapeCsvValue("Weak Categories"));
            csv.AppendLine("Category,SoldCount");

            foreach (var data in report.WeakBrands)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(data.BrandName)},{CsvHelper.EscapeCsvValue(data.SoldCount.ToString())}");
            }

            return csv.ToString();
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("CSV/DemographicsInfo")]
        public async Task<FileContentResult?> GetCsvDemographicsInfo([FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null)
        {
            try
            {
                var (parsedStartDate, parsedEndDate) = DateTimeHelper.ParseDates(startDate, endDate);

                var topCities = await reportService.GetTopCities(parsedStartDate, parsedEndDate);
                var csv = ConvertToCsvDemographicsInfo(topCities);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "demographics.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string ConvertToCsvDemographicsInfo(List<SellingCityServiceModel> data)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Country,City,SoldCount");

            foreach (var entity in data)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(entity.Country)},{CsvHelper.EscapeCsvValue(entity.City)},{CsvHelper.EscapeCsvValue(entity.SoldCount.ToString())}");
            }

            return csv.ToString();
        }
    }
}
