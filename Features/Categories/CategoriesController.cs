namespace NutriBest.Server.Features.Categories
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Categories.Models;
    using NutriBest.Server.Utilities;
    using System.Text;

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

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("CSV")]
        public async Task<FileContentResult?> GetCsv()
        {
            try
            {
                var categories = await categoryService.All();
                var csv = ConvertToCsv(categories);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "categories.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string ConvertToCsv(IEnumerable<CategoryServiceModel> categories)
        {
            var csv = new StringBuilder();
            csv.AppendLine("CategoryName");

            foreach (var category in categories)
            {
                csv.AppendLine(CsvHelper.EscapeCsvValue(category.Name ?? ""));
            }

            return csv.ToString();
        }
    }
}
