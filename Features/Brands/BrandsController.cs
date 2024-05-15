namespace NutriBest.Server.Features.Brands
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Brands.Models;

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
        [Route("/products/by-brand-count")]
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
    }
}
