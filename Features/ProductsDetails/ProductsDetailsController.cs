using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.ProductsDetails
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using NutriBest.Server.Features.Products.Models;
    using NutriBest.Server.Features.ProductsDetails.Models;
    using NutriBest.Server.Shared.Responses;
    using static ErrorMessages.ProductsController;

    public class ProductsDetailsController : ApiController
    {
        private readonly IProductsDetailsService productDetailsService;

        public ProductsDetailsController(IProductsDetailsService productDetailsService)
            => this.productDetailsService = productDetailsService;


        [HttpGet]
        [Route("/Products/Details/{id}/{name}")]
        public async Task<ActionResult<ProductDetailsServiceModel>> GetByProductId([FromRoute] int id,
            [FromRoute] string name)
        {
            try
            {
                var product = await productDetailsService.GetById(id);

                if (product.Name != name)
                    return BadRequest(new FailResponse
                    {
                        Message = InvalidProduct
                    });

                return Ok(product);
            }
            catch (ArgumentNullException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.ParamName != null ?
                    err.ParamName :
                    ""
                });
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
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/Products/Details/{id}")]
        public async Task<ActionResult<bool>> SetDetails([FromRoute] int id,
            [FromForm] CreateProductDetailsServiceModel details)
        {
            try
            {
                var product = await productDetailsService.GetById(id);

                await productDetailsService.AddDetails(id,
                    details.HowToUse,
                    details.ServingSize,
                    details.WhyChoose,
                    details.Ingredients);

                return Ok(true);
            }
            catch (Exception)
            {
                return BadRequest(false);
            }
        }
    }
}
