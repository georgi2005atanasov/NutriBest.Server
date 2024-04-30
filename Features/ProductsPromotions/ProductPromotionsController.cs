namespace NutriBest.Server.Features.ProductsPromotions
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.ProductsPromotions.Models;

    public class ProductPromotionsController : ApiController
    {
        private readonly IProductPromotionService productPromotionService;

        public ProductPromotionsController(IProductPromotionService productPromotionService)
        {
            this.productPromotionService = productPromotionService;
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/add-product-promotion")]
        public async Task<ActionResult<bool>> Create(ProductPromotionServiceModel productPromotion)
        {
            try
            {
                var promotion = await productPromotionService.Create(productPromotion.ProductId,
                productPromotion.PromotionId);

                return Ok(true);
            }
            catch (ArgumentNullException err)
            {
                return BadRequest(new
                {
                    err.Message
                });
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
                    Message = "Could not create a new promotion for the product!"
                });
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/remove-product-promotion")]
        public async Task<ActionResult<bool>> Remove(ProductPromotionServiceModel productPromotion)
        {
            try
            {
                var promotion = await productPromotionService.Create(productPromotion.ProductId,
                productPromotion.PromotionId);

                return Ok(true);
            }
            catch (ArgumentNullException err)
            {
                return BadRequest(new
                {
                    err.Message
                });
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
                    Message = "Could not create a new promotion for the product!"
                });
            }
        }
    }
}
