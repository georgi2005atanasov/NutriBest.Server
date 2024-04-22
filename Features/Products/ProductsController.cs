namespace NutriBest.Server.Features.Products
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Categories;
    using NutriBest.Server.Features.Images;
    using NutriBest.Server.Features.Products.Models;

    public class ProductsController : ApiController
    {
        private readonly NutriBestDbContext db;
        private readonly IProductService productService;
        private readonly IImageService imageService;
        private readonly ICategoryService categoryService;
        private readonly IMemoryCache memoryCache;

        public ProductsController(IProductService productService,
            NutriBestDbContext db,
            IImageService imageService,
            ICategoryService categoryService,
            IMemoryCache memoryCache)
        {
            this.db = db;
            this.productService = productService;
            this.imageService = imageService;
            this.categoryService = categoryService;
            this.memoryCache = memoryCache;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult> Create([FromForm] CreateProductRequestModel productModel)
        {
            var passedCategories = productModel.Categories[0]
                .Split(",")
                .ToList();

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
                .GetCategoriesIds(passedCategories);

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IEnumerable<ProductListingModel>>>> All(
            [FromQuery] int page = 1, 
            [FromQuery] string? categories = "", 
            [FromQuery] string? price = "",
            [FromQuery] string? alpha = "",
            [FromQuery] string? productsView = "all",
            [FromQuery] string? search = "",
            [FromQuery] string? priceRange = "") //might add from the query filters
        {
            Console.WriteLine(priceRange);
            if (page < 1)
            {
                return BadRequest();
            }

            if (productsView == "table" && !User.IsInRole("Administrator"))
            {
                return BadRequest();
            }

            string cacheKey = $"products_page_{page}_categories_{categories}_price_{price}_alpha_{alpha}_search_{search}";
            if (!memoryCache.TryGetValue(cacheKey, out IEnumerable<IEnumerable<ProductListingModel>> cachedProducts))
            {
                var products = await productService.All(page, categories, price, alpha, productsView, search, priceRange);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Sets the time the cache entry can be inactive (not accessed) before it will be removed.
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1)); // Sets a fixed time to live for the cache entry

                memoryCache.Set(cacheKey, products, cacheEntryOptions);
                return Ok(products);
            }

            return Ok(cachedProducts);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ProductListingModel>> Details([FromRoute] int id)
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

            if (await ProductExists(productModel.Name) && product!.Name != productModel.Name)
            {
                return BadRequest(new
                {
                    Key = "Name",
                    Message = "Product with this name already exists!"
                });
            }

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

                memoryCache.Remove($"image_{productModel.ProductId}");

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

        [Authorize(Roles = "Administrator")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<bool>> Delete([FromRoute] int id)
        {
            var product = await db.Products
                .FirstOrDefaultAsync(x => x.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            var result = await productService.Delete(id);

            memoryCache.Remove($"image_{id}");

            return result;
        }

        [HttpGet]
        [Route("by-category-count")]
        public async Task<ActionResult<bool>> GetByCategoryCount()
        {
            var products = await categoryService.GetProductsCountByCategory();

            return Ok(products);
        }

        //[Authorize("Administrator")]
        [HttpGet]
        [Route("identifiers")]
        public async Task<ActionResult<IEnumerable<int>>> GetProductsIds()
            => Ok(await db.Products
            .Select(x => x.ProductId)
            .OrderBy(x => x)
            .ToListAsync());

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
