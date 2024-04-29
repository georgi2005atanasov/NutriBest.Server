namespace NutriBest.Server.Features.ProductsPromotions
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.ProductsPromotions.Models;

    public class ProductPromotionsController : ApiController
    {
        private readonly IProductPromotionService promotionService;

        public ProductPromotionsController(IProductPromotionService promotionService)
        {
            this.promotionService = promotionService;
        }

        [HttpGet]
        [Route("/promotions/{promotionId}")]
        public async Task<ActionResult> Get([FromRoute] int promotionId)
        {
            try
            {
                var promotion = await promotionService.Get(promotionId);

                return Ok(promotion);
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
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/promotions/{productId}")]
        public async Task<ActionResult> Create([FromRoute] int productId,
            CreatePromotionServiceModel promotion) // may receive it from a form
        {
            if (promotion.StartDate > promotion.EndDate)
            {
                return BadRequest(new
                {
                    Key = "StartDate",
                    Message = "The start date must be before the end date!"
                });
            }

            if (promotion.DiscountPercentage == null &&
                promotion.DiscountAmount == null &&
                promotion.SpecialPrice == null)
            {
                return BadRequest(new
                {
                    Key = "SpecialPrice",
                    Message = "You have to make some kind of discount!"
                });
            }

            try
            {
                var result = await promotionService.Create(productId,
                    promotion.Description,
                    promotion.DiscountAmount,
                    promotion.DiscountPercentage,
                    promotion.StartDate,
                    promotion.EndDate,
                    promotion.SpecialPrice); //set is active = true there

                return Ok();
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
                return BadRequest();
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/promotions/{promotionId}")]
        public async Task<ActionResult<bool>> Remove([FromRoute] int promotionId)
        {
            try
            {
                var result = await promotionService.Remove(promotionId);

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
