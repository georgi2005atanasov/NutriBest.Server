namespace NutriBest.Server.Features.NutritionsFacts
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.NutritionsFacts.Models;
    using NutriBest.Server.Features.Products;

    public class NutritionFactsController : ApiController
    {
        private readonly IProductService productService;
        private readonly INutritionFactsService nutritionFactsService;

        public NutritionFactsController(IProductService productService,
            INutritionFactsService nutritionFactsService)
        {
            this.productService = productService;
            this.nutritionFactsService = nutritionFactsService;
        }

        [HttpGet]
        [Route("/products/nutri-facts/{id}")]
        public async Task<ActionResult<NutritionFactsServiceModel>> Facts([FromRoute] int id,
            [FromQuery] string name)
        {
            try
            {
                var facts = await nutritionFactsService.Get(id, name);

                return Ok(facts);
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new
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
        [Route("/products/nutri-facts/{id}")]
        public async Task<ActionResult> SetFacts([FromRoute] int id,
            [FromQuery] string name, [FromBody] NutritionFactsServiceModel details)
        {
            try
            {
                var product = await productService.GetById(id, name);

                await nutritionFactsService.Add(id,
                    details.Proteins,
                    details.Sugars,
                    details.Carbohydrates,
                    details.Fats,
                    details.SaturatedFats,
                    details.EnergyValue,
                    details.Salt);

                return Ok(true);
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new
                {
                    Message = err.Message
                });
            }
            catch (Exception)
            {
                return BadRequest(false);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/products/nutri-facts/{id}")]
        public async Task<ActionResult<bool>> RemoveDetails([FromRoute] int id,
            [FromQuery] string name)
        {
            try
            {
                var product = await productService.GetById(id, name);

                await nutritionFactsService.Remove(id, name);

                return Ok(true);
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
                return BadRequest(false);
            }
        }
    }
}
