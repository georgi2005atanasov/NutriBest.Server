using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Promotions
{
    using System.Globalization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using NutriBest.Server.Features.Products.Models;
    using NutriBest.Server.Features.Promotions.Models;
    using NutriBest.Server.Shared.Responses;
    using static ErrorMessages.PromotionsController;

    public class PromotionsController : ApiController
    {
        private readonly IPromotionService promotionService;

        public PromotionsController(IPromotionService promotionService) 
            => this.promotionService = promotionService;

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
        [Route("{promotionId}")]
        public async Task<ActionResult> Get([FromRoute] int promotionId)
        {
            try
            {
                var promotion = await promotionService.Get(promotionId);

                return Ok(promotion);
            }
            catch (ArgumentNullException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.Message
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
        public async Task<ActionResult> Create([FromForm] CreatePromotionServiceModel promotion) // may receive it from a form
        {
            var (discountAmount, discountPercentage) = ValidatePromotionPrices(promotion.DiscountAmount,
                promotion.DiscountPercentage);

            if (discountPercentage >= 100)
                return BadRequest(new FailResponse
                {
                    Message = InvalidDiscount
                });

            if (promotion.StartDate > promotion.EndDate)
                return BadRequest(new FailResponse
                {
                    Key = "StartDate",
                    Message = StartDateMustBeBeforeEndDate
                });

            if (promotion.EndDate < DateTime.UtcNow)
                return BadRequest(new FailResponse
                {
                    Key = "EndDate",
                    Message = LeastPromotionDurationRequired
                });

            if (discountPercentage < 0 ||
                discountAmount < 0)
                return BadRequest(new FailResponse
                {
                    Message = InvalidDiscount
                });

            if (promotion.DiscountPercentage == null &&
                promotion.DiscountAmount == null)
                return BadRequest(new FailResponse
                {
                    Message = DiscountIsRequired
                });

            if (promotion.DiscountPercentage != null &&
                promotion.DiscountAmount != null)
                return BadRequest(new FailResponse
                {
                    Message = TypeOfDiscountIsRequired
                });

            try
            {
                var result = await promotionService.Create(promotion.Description,
                    discountAmount,
                    discountPercentage,
                    promotion.StartDate,
                    promotion.EndDate,
                    promotion.Category,
                    promotion.Brand);

                return Ok(result);
            }
            catch (ArgumentException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.Message
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

        [HttpPut]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/Promotions/{promotionId}")]
        public async Task<ActionResult<bool>> Update([FromRoute] int promotionId, [FromForm] UpdatePromotionServiceModel promotion) // may receive it from a form
        {
            var (discountAmount, discountPercentage) = ValidatePromotionPrices(promotion.DiscountAmount,
                promotion.DiscountPercentage
                );

            if (promotion.StartDate > promotion.EndDate)
                return BadRequest(new FailResponse
                {
                    Key = "StartDate",
                    Message = StartDateMustBeBeforeEndDate
                });

            if (promotion.EndDate < DateTime.UtcNow)
                return BadRequest(new FailResponse
                {
                    Key = "EndDate",
                    Message = LeastPromotionDurationRequired
                });

            try
            {
                var result = await promotionService.Update(promotionId,
                    promotion.Description,
                    discountAmount,
                    discountPercentage,
                    promotion.Category,
                    promotion.Brand,
                    promotion.StartDate,
                    promotion.EndDate);

                return Ok(result);
            }
            catch (ArgumentException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.Message
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

        [HttpDelete]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/Promotions/{promotionId}")]
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

        [HttpPut]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/Promotions/Status/{promotionId}")]
        public async Task<ActionResult<bool>> ChangePromotionStatus([FromRoute] int promotionId)
        {
            try
            {
                var result = await promotionService.ChangeIsActive(promotionId);

                return Ok(result);
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.Message
                });
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
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("{promotionId}/Products")]
        public async Task<ActionResult<List<ProductServiceModel>>> GetProductsOfPromotion([FromRoute] int promotionId)
        {
            try
            {
                var products = await promotionService.GetProductsOfPromotion(promotionId);

                return Ok(products);
            }
            catch (ArgumentNullException err)
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

        private (decimal? discountAmount, decimal? discountPercentage)
            ValidatePromotionPrices(string? promoDiscountAmount, string? promoDiscountPercentage)
        {
            decimal? amount = null;
            decimal? percentage = null;

            if (!string.IsNullOrEmpty(promoDiscountAmount) &&
                decimal.TryParse(promoDiscountAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal discountAmount))
            {
                amount = discountAmount;
            }
            else if (!string.IsNullOrEmpty(promoDiscountAmount) &&
                !decimal.TryParse(promoDiscountAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal check))
            {
                throw new InvalidOperationException(InvalidDiscount);
            }


            if (!string.IsNullOrEmpty(promoDiscountPercentage) && 
                decimal.TryParse(promoDiscountPercentage, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal discountPercentage))
            {
                percentage = discountPercentage;
            }
            else if (!string.IsNullOrEmpty(promoDiscountPercentage) &&
                !decimal.TryParse(promoDiscountPercentage, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal check))
            {
                throw new InvalidOperationException(InvalidDiscount);
            }

            return (amount, percentage);
        }
    }
}
