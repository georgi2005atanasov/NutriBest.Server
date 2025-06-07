using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Brands
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using NutriBest.Server.Features.Brands.Models;
    using NutriBest.Server.Shared.Responses;
    using static ErrorMessages.BrandsController;

    public class BrandsController : ApiController
    {
        private readonly IBrandService brandService;

        public BrandsController(IBrandService brandService) 
            => this.brandService = brandService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandServiceModel>>> All()
        {
            try
            {
                var brands = await brandService.All();

                return Ok(brands);
            }
            catch (Exception)
            {
                return BadRequest(new FailResponse
                {
                    Message = CouldNotGetBrands
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
                return BadRequest(new FailResponse
                {
                    Message = err.Message
                });
            }
            catch (Exception)
            {
                return BadRequest(new FailResponse
                {
                    Message = CouldNotCreateBrands
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
            catch (ArgumentNullException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.ParamName ?? ErrorMessages.Exception
                });
            }
            catch (Exception)
            {
                return BadRequest(new FailResponse
                {
                    Message = ErrorMessages.Exception
                });
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
                return BadRequest(new FailResponse
                {
                    Message = ErrorMessages.Exception
                });
            }
        }
    }
}
