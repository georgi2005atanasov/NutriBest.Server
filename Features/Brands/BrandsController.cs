namespace NutriBest.Server.Features.Brands
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Brands.Models;
    using NutriBest.Server.Utilities;
    using System.Text;

    public class BrandsController : ApiController
    {
        private readonly IBrandService brandService;

        public BrandsController(IBrandService brandService)
        {
            this.brandService = brandService;
        }

        [HttpGet]
        //[ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByHeader = "User-Agent")]
        public async Task<ActionResult<IEnumerable<BrandServiceModel>>> All()
        {
            try
            {
                var brands = await brandService.All();

                return Ok(brands);
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    Message = "Could not get the brands!"
                });
            }
        }

        [HttpGet]
        [Route("{name}")]
        //[ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByHeader = "User-Agent")]
        public async Task<ActionResult<BrandServiceModel>> Get(string name)
        {
            try
            {
                var brand = await brandService.Get(name);

                if (brand == null)
                    return BadRequest(new
                    {
                        Message = "Invalid brand name!"
                    });

                return Ok(brand);
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
                return BadRequest(new
                {
                    Message = "Invalid brand name!"
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult<int>> Create([FromForm] CreateBrandServiceModel brandModel)
        {
            try
            {
                var brandId = await brandService.Create(brandModel.Name,
                    brandModel.Description,
                    brandModel.Image);

                return Ok(brandId);
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
                return BadRequest(new
                {
                    Message = "Could not create brand!"
                });
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("{name}")]
        public async Task<ActionResult<bool>> Remove([FromRoute] string name)
        {
            try
            {
                var result = await brandService.Remove(name);

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/Products/ByBrandCount")]
        public async Task<ActionResult<List<BrandCountServiceModel>>> GetProductsByBrandCount()
        {
            try
            {
                var products = await brandService.GetProductsByBrandCount();

                return Ok(products);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("CSV")]
        public async Task<FileContentResult?> GetCsv()
        {
            try
            {
                var brands = await brandService.All();
                var csv = ConvertToCsv(brands);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "brands.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string ConvertToCsv(IEnumerable<BrandServiceModel> brands)
        {
            var csv = new StringBuilder();
            csv.AppendLine("BrandName,Description");

            foreach (var brand in brands)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(brand.Name ?? "")},{CsvHelper.EscapeCsvValue(brand.Description ?? "-")}");
            }

            return csv.ToString();
        }
    }
}
