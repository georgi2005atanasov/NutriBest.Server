using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriBest.Server.Data;
using NutriBest.Server.Features.Categories.Models;

namespace NutriBest.Server.Features.Categories
{
    public class CategoriesController : ApiController
    {
        private readonly NutriBestDbContext db;
        private readonly ICategoryService categoryService;

        public CategoriesController(NutriBestDbContext db, ICategoryService categoryService)
        {
            this.db = db;
            this.categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<CategoryServiceModel>> All()
        {
            try
            {
                var categories = await db.Categories
                    .Select(x => new CategoryServiceModel
                    {
                        Name = x.Name
                    })
                    .ToListAsync();

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
    }
}
