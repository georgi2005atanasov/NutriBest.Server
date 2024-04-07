using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriBest.Server.Data;
using NutriBest.Server.Data.Models;
using NutriBest.Server.Models.Products;

namespace NutriBest.Server.Controllers
{
    public class ProductsController : ApiController
    {
        private readonly NutriBestDbContext db;

        public ProductsController(NutriBestDbContext db)
        {
            this.db = db;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [Route(nameof(Create))]
        public async Task<ActionResult> Create([FromForm] CreateProductRequestModel productModel)
        {
            var product = new Product
            {
                Name = productModel.Name,
                Description = productModel.Description,
                Price = productModel.Price
            };

            if (productModel.Image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await productModel.Image.CopyToAsync(memoryStream);
                    product.Image = memoryStream.ToArray();
                }

                db.Products.Add(product);
                await db.SaveChangesAsync();

                return Created(nameof(Create), product.Id);
            }
            else
            {
                return BadRequest("Image is required");
            }
        }
    }
}
