using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriBest.Server.Features.PromoCodes.Models;

namespace NutriBest.Server.Features.PromoCodes
{
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
            if (promoCodeModel.DiscountPercentage >= 100 ||
                promoCodeModel.DiscountPercentage < 0.1m)
            {
                return BadRequest(new
                {
                    Message = "Discount percentage must be betweeen 0% and 99.9%!"
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
    }
}
