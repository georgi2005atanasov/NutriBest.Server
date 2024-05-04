namespace NutriBest.Server.Features.Promotions
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.Promotions.Models;
    using System.Globalization;

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
            catch (Exception err)
            {
                return BadRequest(err.Message);
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
        public async Task<ActionResult> Create([FromForm] CreatePromotionServiceModel promotion) // may receive it from a form
        {
            var (discountAmount, discountPercentage, minimumPrice) = ValidatePromotionPrices(promotion.DiscountAmount,
                promotion.DiscountPercentage,
                promotion.MinimumPrice
                );

            if (discountPercentage >= 100)
            {
                return BadRequest(new
                {
                    Message = "The discount cannot be more than 99.9%!"
                });
            }

            if (promotion.StartDate > promotion.EndDate)
                return BadRequest(new
                {
                    Key = "StartDate",
                    Message = "The start date must be before the end date!"
                });

            if (promotion.EndDate < DateTime.UtcNow)
                return BadRequest(new
                {
                    Key = "EndDate",
                    Message = "The end date must be at least with one day duration!"
                });

            if (discountPercentage < 0 ||
                discountAmount < 0)
                return BadRequest(new
                {
                    Message = "Invalid discount!"
                });

            if (promotion.DiscountPercentage == null &&
                promotion.DiscountAmount == null)
                return BadRequest(new
                {
                    Message = "You have to make some kind of discount!"
                });

            if (promotion.DiscountPercentage != null &&
                promotion.DiscountAmount != null)
                return BadRequest(new
                {
                    Message = "You have to choose one type of discount!"
                });

            try
            {
                var result = await promotionService.Create(promotion.Description,
                    discountAmount,
                    discountPercentage,
                    promotion.StartDate,
                    promotion.EndDate,
                    minimumPrice,
                    promotion.Category);

                return Ok(result);
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
                return BadRequest();
            }
        }

        [HttpPut]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/promotions/{promotionId}")]
        public async Task<ActionResult> Update([FromRoute] int promotionId, [FromForm] UpdatePromotionServiceModel promotion) // may receive it from a form
        {
            var (discountAmount, discountPercentage, minimumPrice) = ValidatePromotionPrices(promotion.DiscountAmount,
                promotion.DiscountPercentage,
                promotion.MinimumPrice
                );

            if (promotion.StartDate > promotion.EndDate)
                return BadRequest(new
                {
                    Key = "StartDate",
                    Message = "The start date must be before the end date!"
                });

            if (promotion.EndDate < DateTime.UtcNow)
                return BadRequest(new
                {
                    Key = "EndDate",
                    Message = "The end date must be at least with one day duration!"
                });

            try
            {
                var result = await promotionService.Update(promotionId,
                    promotion.Description,
                    discountAmount,
                    discountPercentage,
                    minimumPrice,
                    promotion.Category,
                    promotion.StartDate,
                    promotion.EndDate);

                return Ok();
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

        [HttpPut]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/promotions/status/{promotionId}")]
        public async Task<ActionResult<bool>> ChangePromotionStatus([FromRoute] int promotionId)
        {
            try
            {
                var result = await promotionService.ChangeIsActive(promotionId);

                return Ok(result);
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new
                {
                    err.Message
                });
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
                return BadRequest();
            }
        }

        private (decimal? discountAmount, decimal? discountPercentage, decimal? minimumPrice)
            ValidatePromotionPrices(string? promoDiscountAmount, string? promoDiscountPercentage, string? promoMinimumPrice)
        {
            decimal? amount = null;
            decimal? percentage = null;
            decimal? minPrice = null;

            if (!string.IsNullOrEmpty(promoDiscountAmount) &&
                decimal.TryParse(promoDiscountAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal discountAmount))
            {
                amount = discountAmount;
            }
            else if (!string.IsNullOrEmpty(promoDiscountAmount) &&
                !decimal.TryParse(promoDiscountAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal check))
            {
                throw new InvalidOperationException("Invalid discount!");
            }


            if (!string.IsNullOrEmpty(promoDiscountPercentage) && 
                decimal.TryParse(promoDiscountPercentage, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal discountPercentage))
            {
                percentage = discountPercentage;
            }
            else if (!string.IsNullOrEmpty(promoDiscountPercentage) &&
                !decimal.TryParse(promoDiscountPercentage, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal check))
            {
                throw new InvalidOperationException("Invalid discount!");
            }

            if (!string.IsNullOrEmpty(promoMinimumPrice) && 
                decimal.TryParse(promoMinimumPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal minimumPrice))
            {
                minPrice = minimumPrice;
            }
            else if (!string.IsNullOrEmpty(promoMinimumPrice) &&
                !decimal.TryParse(promoMinimumPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal check))
            {
                throw new InvalidOperationException("Invalid minimum price!");
            }

            return (amount, percentage, minPrice);
        }

    }
}
