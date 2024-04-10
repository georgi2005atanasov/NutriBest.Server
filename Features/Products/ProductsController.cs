namespace NutriBest.Server.Features.Products
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Products.Models;

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
        public async Task<ActionResult<IEnumerable<ProductListingModel>>> All(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10) //might add from the query filters
        {
            var products = await productService.All();

            return Ok(products);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<IEnumerable<ProductListingModel>>> Details([FromRoute] int id)
        {
            var products = await productService.GetById(id);

            return Ok(products);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut]
        public async Task<ActionResult<int>> Update([FromRoute] int id)
        {
            var products = await productService.GetById(id);

        }
    }
}
