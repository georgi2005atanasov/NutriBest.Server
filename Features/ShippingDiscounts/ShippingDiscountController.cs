namespace NutriBest.Server.Features.ShippingDiscounts
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.ShippingDiscounts.Models;
    using System.Globalization;
    using static ServicesConstants.Promotion;

    public class ShippingDiscountController : ApiController
    {
        private readonly IShippingDiscountService shippingDiscountService;

        public ShippingDiscountController(IShippingDiscountService shippingDiscountService)
        {
            this.shippingDiscountService = shippingDiscountService;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("All")]
        public async Task<ActionResult<ShippingDiscountServiceModel>> All()
        {
            try
            {
                var shippingDiscounts = await shippingDiscountService.All();

                return Ok(shippingDiscounts);
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
        [Route("{countryName}")]
        public async Task<ActionResult<ShippingDiscountServiceModel>> Get([FromRoute] string countryName)
        {
            try
            {
                var shippingDiscount = await shippingDiscountService.Get(countryName);

                return Ok(shippingDiscount);
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


        [HttpPost]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult<int>> Create([FromBody] CreateShippingDiscountServiceModel shippingDiscountModel)
        {
            if (!decimal.TryParse(shippingDiscountModel.DiscountPercentage, NumberStyles.Any, CultureInfo.InvariantCulture, out var discountPercentage))
                return BadRequest(new
                {
                    Key = "DiscountPercentage",
                    Message = "The Discount Percentage must be between 1 and 100!"
                });

            if (!string.IsNullOrEmpty(shippingDiscountModel.MinimumPrice) &&
                !decimal.TryParse(shippingDiscountModel.DiscountPercentage, NumberStyles.Any, CultureInfo.InvariantCulture, out var minimumPrice))
                return BadRequest(new
                {
                    Key = "MinimumPrice",
                    Message = "Prices must be numbers!"
                });

            if (MinPercentage >= discountPercentage || MaxPercentage < discountPercentage)
                return BadRequest(new
                {
                    Key = "DiscountPercentage",
                    Message = "The Discount Percentage must be between 1 and 100!"
                });

            if (string.IsNullOrEmpty(shippingDiscountModel.Description) ||
                shippingDiscountModel.Description.Length > MaxDescriptionLength ||
                shippingDiscountModel.Description.Length < MinDescriptionLength)
                return BadRequest(new
                {
                    Key = "Description",
                    Message = "Description length must be between 5 and 50!"
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
            catch (Exception err)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult<bool>> Delete([FromBody] DeleteShippingDiscountServiceModel shippingDiscountModel)
        {
            try
            {
                var result = await shippingDiscountService.Delete(shippingDiscountModel.CountryName);

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
