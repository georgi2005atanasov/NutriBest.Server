using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriBest.Server.Data;
using NutriBest.Server.Data.Models;
using NutriBest.Server.Features.PromoCodes.Extensions;
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
        public async Task<ActionResult<List<string>>> Create([FromBody] PromoCodeServiceModel promoCodeModel)
        {
            if (promoCodeModel.DiscountPercentage == null && promoCodeModel.DiscountAmount == null)
            {
                return BadRequest(new
                {
                    Message = "You have to make some type of discount!"
                });
            }

            if (promoCodeModel.DiscountPercentage != null && promoCodeModel.DiscountAmount != null)
            {
                return BadRequest(new
                {
                    Message = "You can make only one type of discount!"
                });
            }

            try
            {
                var codes = await promoCodeService.Create(promoCodeModel.DiscountAmount,
                    promoCodeModel.DiscountPercentage,
                    promoCodeModel.Count);

                return Ok(codes);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
