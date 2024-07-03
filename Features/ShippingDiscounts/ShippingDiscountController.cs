using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.ShippingDiscounts
{
    using System.Globalization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using NutriBest.Server.Features.ShippingDiscounts.Models;
    using NutriBest.Server.Shared.Responses;
    using static ServicesConstants.Promotion;
    using static ErrorMessages.ShippingDiscountController;

    public class ShippingDiscountController : ApiController
    {
        private readonly IShippingDiscountService shippingDiscountService;

        public ShippingDiscountController(IShippingDiscountService shippingDiscountService)
        {
            this.shippingDiscountService = shippingDiscountService;
        }

        [HttpGet]
        [Route(nameof(All))]
        public async Task<ActionResult<AllShippingDiscountsServiceModel>> All()
        {
            try
            {
                var shippingDiscounts = await shippingDiscountService.All();

                return Ok(shippingDiscounts);
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

        [HttpPost]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult<int>> Create([FromBody] CreateShippingDiscountServiceModel shippingDiscountModel)
        {
            if (!decimal.TryParse(shippingDiscountModel.DiscountPercentage, NumberStyles.Any, CultureInfo.InvariantCulture, out var discountPercentage))
                return BadRequest(new FailResponse
                {
                    Key = "DiscountPercentage",
                    Message = InvalidDiscountPercentage
                });

            if (!string.IsNullOrEmpty(shippingDiscountModel.MinimumPrice) &&
                !decimal.TryParse(shippingDiscountModel.MinimumPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out var minimumPrice))
                return BadRequest(new FailResponse
                {
                    Key = "MinimumPrice",
                    Message = PricesMustBeNumbers
                });

            if (MinPercentage >= discountPercentage || MaxPercentage < discountPercentage)
                return BadRequest(new FailResponse
                {
                    Key = "DiscountPercentage",
                    Message = InvalidDiscountPercentage
                });

            if (string.IsNullOrEmpty(shippingDiscountModel.Description) ||
                shippingDiscountModel.Description.Length > MaxDescriptionLength ||
                shippingDiscountModel.Description.Length < MinDescriptionLength)
                return BadRequest(new
                {
                    Key = "Description",
                    Message = InvalidDescriptionLength
                });

            try
            {
                var shippingDiscountId = await shippingDiscountService.Create(shippingDiscountModel.CountryName,
                    discountPercentage,
                    shippingDiscountModel.EndDate,
                    shippingDiscountModel.Description,
                    shippingDiscountModel.MinimumPrice);

                return Ok(new
                {
                    Id = shippingDiscountId
                });
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
        public async Task<ActionResult<bool>> Remove([FromBody] DeleteShippingDiscountServiceModel shippingDiscountModel)
        {
            try
            {
                var result = await shippingDiscountService
                    .Remove(shippingDiscountModel.CountryName);

                return Ok(new
                {
                    Succeeded = result
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
