using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.PromoCodes
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using NutriBest.Server.Features.PromoCodes.Models;
    using NutriBest.Server.Shared.Responses;
    using static ServicesConstants.PromoCodes;
    using static ErrorMessages.PromoCodeController;

    public class PromoCodeController : ApiController
    {
        private readonly IPromoCodeService promoCodeService;

        public PromoCodeController(IPromoCodeService promoCodeService) 
            => this.promoCodeService = promoCodeService;

        [HttpPost]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult<Dictionary<string, List<string>>>> Create([FromBody] PromoCodeServiceModel promoCodeModel)
        {
            try
            {
                decimal discountPercentage = 0;
                int count = 0;

                if (!decimal.TryParse(promoCodeModel.DiscountPercentage, out discountPercentage))
                    return BadRequest(new FailResponse
                    {
                        Key = "DiscountPercentage",
                        Message = EnterValidPercentage
                    });

                if (!int.TryParse(promoCodeModel.Count, out count))
                    return BadRequest(new FailResponse
                    {
                        Key = "Count",
                        Message = "Enter some positive number!"
                    });

                if (discountPercentage > (decimal)MaxDiscount ||
                    discountPercentage < (decimal)MinDiscount)
                {
                    return BadRequest(new FailResponse
                    {
                        Message = EnterValidPercentage
                    });
                }

                if (count <= 0)
                {
                    return BadRequest(new FailResponse
                    {
                        Message = "The count must be a positive number!"
                    });
                }
                var codes = await promoCodeService.Create(discountPercentage,
                    count,
                    promoCodeModel.Description);

                var result = new Dictionary<string, List<string>>();
                result[promoCodeModel.Description] = codes;

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest(new FailResponse
                {
                    Message = "Invalid request!"
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<PromoCodeByDescriptionServiceModel>>> All()
        {
            try
            {
                var promoCodes = await promoCodeService.All();
                return Ok(promoCodes);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        //[HttpGet]
        //[Route("ByCode")]
        //public async Task<ActionResult<PromoCodeListingModel>> GetByCode([FromQuery] string code)
        //{
        //    try
        //    {
        //        var promoCode = await promoCodeService.GetByCode(code);

        //        return Ok(promoCode);
        //    }
        //    catch (ArgumentNullException err)
        //    {
        //        return BadRequest(new
        //        {
        //            err.Message
        //        });
        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest();
        //    }
        //}

        //[HttpGet]
        //[Route("ByDescription")]
        //public async Task<ActionResult<Dictionary<string, List<string>>>> GetByDescription([FromForm] string description)
        //{
        //    try
        //    {
        //        (List<string> promoCodes, int expireIn) = await promoCodeService.GetByDescription(description);

        //        var result = new PromoCodeByDescriptionServiceModel
        //        {
        //            Description = description,
        //            PromoCodes = promoCodes,
        //            ExpireIn = expireIn
        //        };

        //        return Ok(result);
        //    }
        //    catch (ArgumentNullException err)
        //    {
        //        return BadRequest(new
        //        {
        //            err.Message
        //        });
        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest();
        //    }
        //}

        [HttpDelete]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult> DisableByDescription([FromQuery] string description)
        {
            try
            {
                var result = await promoCodeService.DisableByDescription(description);

                return Ok(new
                {
                    Result = result
                });
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    Result = false
                });
            }
        }
    }
}
