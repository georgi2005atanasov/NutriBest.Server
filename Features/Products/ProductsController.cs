using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriBest.Server.Data;
using NutriBest.Server.Data.Enums;
using NutriBest.Server.Features.Products.Models;

namespace NutriBest.Server.Features.Products
{
    public class ProductsController : ApiController
    {
        private readonly IProductService productService;
        private readonly NutriBestDbContext db;

        public ProductsController(IProductService productService, NutriBestDbContext db)
        {
            this.productService = productService;
            this.db = db;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult> Create([FromForm] CreateProductRequestModel productModel)
        {
            if (productModel.Price <= 0)
            {
                return BadRequest("Price must be bigger than zero!");
            }

            var categoriesIds = await productService
                .GetCategoriesIds(productModel.Categories);

            if (categoriesIds.Count == 0)
            {
                return BadRequest("You have to choose at least 1 category!");
            }
            
            if (productModel.Image != null)
            {
                var productImage = await productService
                    .GetImage(productModel.Image, productModel.Image.ContentType);

                var productId = await productService
                    .Create(productModel.Name,
                    productModel.Description,
                    productModel.Price,
                    categoriesIds,
                    productImage.ImageData,
                    productImage.ContentType
                    );

                return Created(nameof(Create), productId);
            }
            else
            {
                return BadRequest("Image is required");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductListingModel>>> All()
        {
            var products = await productService.All();

            return Ok(products);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<IEnumerable<ProductListingModel>>> Details(int id)
        {
            var products = await productService.All();

            if (products == null)
            {
                return NotFound();
            }

            return Ok(products);
        }
    }
}
