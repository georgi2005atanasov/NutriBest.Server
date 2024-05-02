namespace NutriBest.Server.Features.Promotions
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.Promotions.Models;

    public class PromotionsController : ApiController
    {
        private readonly IPromotionService promotionService;

        public PromotionsController(IPromotionService promotionService)
        {
            this.promotionService = promotionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PromotionServiceModel>>> All()
        {
            try
            {
                var promotions = await promotionService.All();

                return Ok(promotions);
            }
            catch (Exception)
            {
                return BadRequest();
            }
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
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult> Create(CreatePromotionServiceModel promotion) // may receive it from a form
        {
            if (promotion.StartDate > promotion.EndDate)
                return BadRequest(new
                {
                    Key = "StartDate",
                    Message = "The start date must be before the end date!"
                });

            if (promotion.DiscountPercentage < 0 ||
                promotion.DiscountAmount < 0)
                return BadRequest(new
                {
                    Message = "Invalid discount!"
                });

            if (promotion.DiscountPercentage == null &&
                promotion.DiscountAmount == null)
                return BadRequest(new
                {
                    Key = "SpecialPrice",
                    Message = "You have to make some kind of discount!"
                });

            if (promotion.DiscountPercentage != null &&
                promotion.DiscountAmount != null)
                return BadRequest(new
                {
                    Key = "SpecialPrice",
                    Message = "You have to choose type of discount!"
                });

            try
            {
                var result = await promotionService.Create(promotion.Description,
                    promotion.DiscountAmount,
                    promotion.DiscountPercentage,
                    promotion.StartDate,
                    promotion.EndDate);

                return Ok(result);
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

        [HttpPut]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/promotions/{promotionId}")]
        public async Task<ActionResult> Update([FromRoute] int promotionId, UpdatePromotionServiceModel promotion) // may receive it from a form
        {
            if (promotion.DiscountPercentage < 0 ||
                promotion.DiscountAmount < 0)
                return BadRequest(new
                {
                    Message = "Invalid discount!"
                });

            try
            {
                var result = await promotionService.Update(promotionId,
                    promotion.Description,
                    promotion.DiscountAmount,
                    promotion.DiscountPercentage);

                return Ok();
            }
            catch(ArgumentException err)
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
