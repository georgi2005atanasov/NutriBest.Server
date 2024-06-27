namespace NutriBest.Server.Features.Categories
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using NutriBest.Server.Features.Categories.Models;

    public class CategoriesController : ApiController
    {
        private readonly ICategoryService categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpGet]
        //[ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByHeader = "User-Agent")]
        public async Task<ActionResult<CategoryServiceModel>> All()
        {
            try
            {
                var categories = await categoryService.All();

                return Ok(categories);
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    Message = "Could not fetch products categories!"
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult<int>> Create([FromForm] CreateCategoryServiceModel categoryModel)
        {
            try
            {
                var categoryId = await categoryService.Create(categoryModel.Name);

                return Ok(categoryId);
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
        [Route("{name}")]
        public async Task<ActionResult<bool>> Remove([FromRoute] string name)
        {
            try
            {
                var result = await categoryService.Remove(name);

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
