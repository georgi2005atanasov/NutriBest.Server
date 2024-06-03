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
            => this.productDetailsService = productDetailsService;


        [HttpGet]
        [Route("/Products/Details/{id}/{name}")]
        public async Task<ActionResult<ProductListingServiceModel>> Details([FromRoute] int id,
            [FromRoute] string name)
        {
            try
            {
                var product = await productDetailsService.GetById(id);

                if (product.Name != name)
                    return BadRequest(new
                    {
                        Message = "Invalid product!"
                    });

                return Ok(product);
            }
            catch (ArgumentNullException err)
            {
                return BadRequest(new
                {
                    Message = err.Message
                });
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
        [Route("/Products/Details/{id}")]
        public async Task<ActionResult<ProductListingServiceModel>> SetDetails([FromRoute] int id,
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
        [Route("/Products/Details/{id}")]
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
