namespace NutriBest.Server.Features.Products
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Categories;
    using NutriBest.Server.Features.Images;
    using NutriBest.Server.Features.Products.Models;

    public class ProductsController : ApiController
    {
        private readonly NutriBestDbContext db;
        private readonly IProductService productService;
        private readonly IImageService imageService;
        private readonly ICategoryService categoryService;

        public ProductsController(IProductService productService,
            NutriBestDbContext db,
            IImageService imageService,
            ICategoryService categoryService)
        {
            this.db = db;
            this.productService = productService;
            this.imageService = imageService;
            this.categoryService = categoryService;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult> Create([FromForm] CreateProductRequestModel productModel)
        {
            if (productModel.Price <= 0)
            {
                return BadRequest("Price must be bigger than zero!");
            }

            var categoriesIds = await categoryService
                .GetCategoriesIds(productModel.Categories);

            if (categoriesIds.Count == 0)
            {
                return BadRequest("You have to choose at least 1 category!");
            }

            if (productModel.Image != null)
            {
                var productImage = await imageService
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
            var product = await productService.GetById(id);

            return Ok(product);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut]
        public async Task<ActionResult<int>> Update([FromForm] UpdateProductModel productModel)
        {
            var product = await db.Products
                .FirstOrDefaultAsync(x => x.ProductId == productModel.ProductId);

            if (product == null)
            {
                return NotFound();
            }

            if (productModel.Price <= 0)
            {
                return BadRequest("Price must be bigger than zero!");
            }

            var categoriesIds = await categoryService
                .GetCategoriesIds(productModel.Categories);

            if (categoriesIds.Count == 0)
            {
                return BadRequest("You have to choose at least 1 category!");
            }

            if (productModel.Image != null)
            {
                var productImage = await imageService
                    .GetImage(productModel.Image, productModel.Image.ContentType);

                int productId = await productService
                    .Update(productModel.ProductId,
                    productModel.Name,
                    productModel.Description,
                    productModel.Price,
                    categoriesIds,
                    productImage.ImageData,
                    productImage.ContentType
                );

                return productId;
            }
            else
            {
                return BadRequest("Image is required!");
            }
        }
    }
}
