namespace NutriBest.Server.Features.ProductsDetails
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.Products;
    using NutriBest.Server.Features.Products.Models;
    using NutriBest.Server.Features.ProductsDetails.Models;

    public class ProductDetailsController : ApiController
    {
        private readonly IProductDetailsService productDetailsService;

        public ProductDetailsController(IProductService productService, IProductDetailsService productDetailsService)
        {
            this.productDetailsService = productDetailsService;
        }

        [HttpGet]
        [Route("/products/details/{id}")]
        public async Task<ActionResult<ProductListingServiceModel>> Details([FromRoute] int id)
        {
            try
            {
                var product = await productDetailsService.GetById(id);

                return Ok(product);
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new
                {
                    Message = err.Message
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/products/details/{id}")]
        public async Task<ActionResult<ProductListingServiceModel>> SetDetails([FromRoute] int id,
            [FromBody] CreateProductDetailsServiceModel details)
        {
            try
            {
                var product = await productDetailsService.GetById(id);

                await productDetailsService.AddDetails(id,
                    details.HowToUse,
                    details.ServingSize,
                    details.ServingsPerContainer);

                return Ok(true);
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new
                {
                    Message = err.Message
                });
            }
            catch (Exception)
            {
                return BadRequest(false);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/products/details/{id}")]
        public async Task<ActionResult<ProductListingServiceModel>> RemoveDetails([FromRoute] int id)
        {
            try
            {
                var product = await productDetailsService.GetById(id);

                await productDetailsService.RemoveDetails(id);

                return Ok(true);
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
                return BadRequest(false);
            }
        }
    }
}
