namespace NutriBest.Server.Features.ProductsPromotions
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class ProductPromotionsController : ApiController
    {
        private readonly IProductPromotionService productPromotionService;

        public ProductPromotionsController(IProductPromotionService productPromotionService)
        {
            this.productPromotionService = productPromotionService;
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/Promotions/add-product-promotion/{promotionId}/{productId}")]
        public async Task<ActionResult<bool>> Create([FromRoute] int promotionId,
            [FromRoute] int productId)
        {
            try
            {
                var promotion = await productPromotionService.Create(productId,
                promotionId);

                return Ok(true);
            }
            catch (ArgumentNullException err)
            {
                return BadRequest(new
                {
                    err.Message
                });
            }
            catch (ArgumentException err)
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
        [Route("/Promotions/remove-product-promotion/{productId}")]
        public async Task<ActionResult<bool>> Remove([FromRoute] int productId)
        {
            try
            {
                var promotion = await productPromotionService.Remove(productId);

                return Ok(true);
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
                    Message = "Could not remove the promotion for the product!"
                });
            }
        }
    }
}
