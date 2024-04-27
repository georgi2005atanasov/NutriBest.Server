using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace NutriBest.Server.Features.Carts
{
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Carts.Models;
    using NutriBest.Server.Infrastructure.Services;

    public class CartsController : ApiController
    {
        private readonly ICartService cartService;

        public CartsController(ICartService cartService)
        {
            this.cartService = cartService;
        }

        [HttpPost]
        [Route("/cart/add")]
        public async Task<ActionResult> Add(CartProductServiceModel cartProductModel)
        {
            try
            {
                var cartProductId = await cartService.Add(cartProductModel.ProductId);

                return Ok(cartProductId);
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new
                {
                    Message = err.Message,
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("/cart/remove")]
        public async Task<ActionResult<bool>> Remove(CartProductServiceModel cartProductModel)
        {
            try
            {
                var result = await cartService.Remove(cartProductModel.ProductId);

                if (!result)
                {
                    return BadRequest(new
                    {
                        Message = "Cannot remove unexisting products from the cart!"
                    });
                }

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("/cart/clean")]
        public async Task<ActionResult> Clean()
        {
            try
            {
                var result = await cartService.Clean();

                if (!result)
                {
                    return BadRequest(new
                    {
                        Message = "The shopping cart is empty!"
                    });
                }

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
