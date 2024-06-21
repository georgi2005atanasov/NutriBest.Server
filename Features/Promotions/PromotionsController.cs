namespace NutriBest.Server.Features.Promotions
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.Products.Models;
    using NutriBest.Server.Features.Promotions.Models;
    using NutriBest.Server.Utilities;
    using System.Globalization;
    using System.Text;

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
        [Route("/Promotions/{promotionId}")]
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
            var (discountAmount, discountPercentage) = ValidatePromotionPrices(promotion.DiscountAmount,
                promotion.DiscountPercentage
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
                    promotion.Category,
                    promotion.Brand);

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
        [Route("/Promotions/{promotionId}")]
        public async Task<ActionResult> Update([FromRoute] int promotionId, [FromForm] UpdatePromotionServiceModel promotion) // may receive it from a form
        {
            var (discountAmount, discountPercentage) = ValidatePromotionPrices(promotion.DiscountAmount,
                promotion.DiscountPercentage
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
                    promotion.Category,
                    promotion.Brand,
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

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("CSV")]
        public async Task<FileContentResult?> GetCsv()
        {
            try
            {
                var promotions = await promotionService.All();
                var csv = ConvertToCsv(promotions);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "products.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string ConvertToCsv(IEnumerable<PromotionServiceModel> promotions)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Id,Description,Brand,Category,DiscountAmount,DiscountPercentage,IsActive,StartDate,EndDate");

            foreach (var promotion in promotions)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(promotion.PromotionId.ToString() ?? "-")},{CsvHelper.EscapeCsvValue(promotion.Description ?? "-")},{CsvHelper.EscapeCsvValue(promotion.Brand ?? "-")},{CsvHelper.EscapeCsvValue(promotion.Category ?? "-")},{CsvHelper.EscapeCsvValue(promotion.DiscountAmount != null ? $"{promotion.DiscountAmount} BGN" : "-")},{CsvHelper.EscapeCsvValue(promotion.DiscountPercentage != null ? $"{promotion.DiscountPercentage}%" : "-")},{CsvHelper.EscapeCsvValue(promotion.IsActive.ToString())},{CsvHelper.EscapeCsvValue(promotion.StartDate.ToString())},{CsvHelper.EscapeCsvValue(promotion.EndDate != null ? $"{promotion.EndDate}" : "")}");
            }

            return csv.ToString();
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

            return (amount, percentage);
        }
    }
}
