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
            if (await ProductExists(productModel.Name))
            {
                return BadRequest(new
                {
                    Key = "Name",
                    Message = "Product with this name already exists!"
                });
            }

            if (productModel.Price <= 0)
            {
                return BadRequest(new
                {
                    Key = "Price",
                    Message = "Price must be bigger than zero!"
                });
            }

            var categoriesIds = await categoryService
                .GetCategoriesIds(productModel.Categories);

            if (categoriesIds.Count == 0)
            {
                return BadRequest(new
                {
                    Key = "Category",
                    Message = "You have to choose at least 1 category!"
                });
            }

            if (productModel.Image != null)
            {
                var productImage = await imageService
                    .CreateImage(productModel.Image, productModel.Image.ContentType);

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
                return BadRequest(new
                {
                    Key = "Image",
                    Message = "Image is required!"
                });
            }
        }

        //products?page=0
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductListingModel>>> All(
            [FromQuery] int page = 0) //might add from the query filters
        {
            var products = await productService.All(page);

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
                return BadRequest(new
                {
                    Key = "Price",
                    Message = "Price must be bigger than zero!"
                });
            }

            var categoriesIds = await categoryService
                .GetCategoriesIds(productModel.Categories);

            if (categoriesIds.Count == 0)
            {
                BadRequest(new
                {
                    Key = "Category",
                    Message = "You have to choose at least 1 category!"
                });
            }

            if (productModel.Image != null)
            {
                var productImage = await imageService
                    .CreateImage(productModel.Image, productModel.Image.ContentType);

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
                return BadRequest(new
                {
                    Key = "Image",
                    Message = "Image is required!"
                });
            }
        }

        private async Task<bool> ProductExists(string productName)
        {
            var products = await db.Products
                .Select(x => x.Name)
                .ToListAsync();

            if (products.Any(name => name == productName))
            {
                return true;
            }

            return false;
        }
    }
}
