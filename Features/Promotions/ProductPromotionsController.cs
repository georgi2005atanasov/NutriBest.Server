namespace NutriBest.Server.Features.Promotions
{
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.Promotions.Models;

    public class ProductPromotionsController : ApiController
    {
        private readonly IProductPromotionService promotionService;

        public ProductPromotionsController(IProductPromotionService promotionService)
        {
            this.promotionService = promotionService;
        }

        //[HttpGet]
        //[Route("/promotions/{productId}")]
        //public async Task<ActionResult> Get([FromRoute] int productId)
        //{

        //}

        [HttpPost]
        [Route("/promotions/{productId}")]
        public async Task<ActionResult> Create([FromRoute] int productId,
            CreatePromotionServiceModel promotion)
        {
            if (promotion.DiscountPercentage == null && 
                promotion.DiscountAmount == null &&
                promotion.SpecialPrice == null)
            {
                return BadRequest(new
                {
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
    }
}
