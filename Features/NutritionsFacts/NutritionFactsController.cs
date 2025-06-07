namespace NutriBest.Server.Features.NutritionsFacts
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Products;
    using NutriBest.Server.Features.NutritionsFacts.Models;
    using NutriBest.Server.Shared.Responses;

    public class NutritionFactsController : ApiController
    {
        private readonly NutriBestDbContext db;
        private readonly IProductService productService;
        private readonly INutritionFactsService nutritionFactsService;

        public NutritionFactsController(NutriBestDbContext db,
            IProductService productService,
            INutritionFactsService nutritionFactsService)
        {
            this.db = db;
            this.productService = productService;
            this.nutritionFactsService = nutritionFactsService;
        }

        [HttpGet]
        [Route("/Products/NutriFacts/{id}/{name}")]
        public async Task<ActionResult<NutritionFactsServiceModel>> All([FromRoute] int id,
            [FromRoute] string name)
        {
            try
            {
                var product = await db.Products
                    .FirstAsync(x => x.ProductId == id);

                if (product.Name != name)
                    return BadRequest(new
                    {
                        Message = "Invalid Product!"
                    });

                var facts = await nutritionFactsService.Get(id);

                return Ok(facts);
            }
            catch (InvalidOperationException err)
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
        [Route("/Products/NutriFacts/{id}")]
        public async Task<ActionResult> SetFacts([FromRoute] int id,
            [FromBody] NutritionFactsServiceModel details)
        {
            try
            {
                var product = await productService.Get(id);

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
    }
}