﻿namespace NutriBest.Server.Features.Categories
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Categories.Models;

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

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateCategoryServiceModel categoryModel) // have to change it to 
                                                                            //[FromForm] !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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
