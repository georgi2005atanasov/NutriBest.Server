namespace NutriBest.Server.Features.PromoCodes
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.PromoCodes.Models;
    using static ServicesConstants.PromoCodes;

    public class PromoCodeController : ApiController
    {
        private readonly IPromoCodeService promoCodeService;

        public PromoCodeController(IPromoCodeService promoCodeService)
        {
            this.promoCodeService = promoCodeService;
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult<Dictionary<string, List<string>>>> Create([FromBody] PromoCodeServiceModel promoCodeModel)
        {
            if (promoCodeModel.DiscountPercentage > (decimal)MaxDiscount ||
                promoCodeModel.DiscountPercentage < (decimal)MinDiscount)
            {
                return BadRequest(new
                {
                    Message = "Discount percentage must be betweeen 0% and 100%!"
                });
            }

            if (promoCodeModel.Count <= 0)
            {
                return BadRequest(new
                {
                    Message = "Choose promo codes count!"
                });
            }

            try
            {
                var codes = await promoCodeService.Create(promoCodeModel.DiscountPercentage,
                    promoCodeModel.Count,
                    promoCodeModel.Description);

                var result = new Dictionary<string, List<string>>();
                result[promoCodeModel.Description] = codes;

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("by-code")]
        public async Task<ActionResult<PromoCodeListingModel>> GetByCode([FromForm] string code)
        {
            try
            {
                var promoCode = await promoCodeService.GetByCode(code);

                return Ok(promoCode);
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
        [Route("by-description")]
        public async Task<ActionResult<Dictionary<string, List<string>>>> GetByDescription([FromForm] string description)
        {
            try
            {
                (List<string> promoCodes, int expireIn) = await promoCodeService.GetByDescription(description);

                var result = new PromoCodeByDescriptionServiceModel
                {
                    Description = description,
                    PromoCodes = promoCodes,
                    ExpireIn = expireIn
                };

                return Ok(result);
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

        [HttpDelete]
        public async Task<ActionResult> DisableByDescription([FromForm] string description)
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
