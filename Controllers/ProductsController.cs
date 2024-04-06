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
        public async Task<ActionResult> Create(CreateProductRequestModel productModel)
        {
            //var userName = this.User.GetId();

            var product = new Product
            {
                Description = productModel.Description,
                Name = productModel.Name
            };

            db.Products.Add(product);

            await db.SaveChangesAsync();

            return Created(nameof(Create), product.Id);
        }
    }
}
