namespace NutriBest.Server.Features.Products
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Categories;
    using NutriBest.Server.Features.Images;
    using NutriBest.Server.Features.Products.Models;
    using System.Globalization;

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

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("{id}")]
        public async Task<ActionResult<ProductListingServiceModel>> GetById([FromRoute] int id)
        {
            try
            {
                var product = await productService.GetById(id);

                return Ok(product);
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new
                {
                    Message = err.Message
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult> Create([FromForm] CreateProductRequestModel productModel)
        {
            try
            {
                var passedCategories = productModel.Categories[0]
                .Split(",")
                .ToList();

                if (ProductExists(productModel.Name))
                {
                    return BadRequest(new
                    {
                        Key = "Name",
                        Message = "Product with this name already exists!"
                    });
                }

                decimal price;

                if (!decimal.TryParse(productModel.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out price))
                {
                    return BadRequest(new
                    {
                        Key = "Price",
                        Message = "Product price must be a number!"
                    });
                }

                if (price <= 0)
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
                        price,
                        productModel.Quantity,
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
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IEnumerable<ProductListingServiceModel>>>> All(
            [FromQuery] int page = 1,
            [FromQuery] string? categories = "",
            [FromQuery] string? price = "",
            [FromQuery] string? alpha = "",
            [FromQuery] string? productsView = "all",
            [FromQuery] string? search = "",
            [FromQuery] string? priceRange = "") //might add from the query filters
        {
            try
            {
                if (page < 1)
                {
                    return BadRequest();
                }

                if (productsView == "table" &&
                    !User.IsInRole("Administrator") && !User.IsInRole("Employee"))
                {
                    return BadRequest();
                }

                string cacheKey = $"products_page_{page}_categories_{categories}_price_{price}_alpha_{alpha}_search_{search}";
                if (!memoryCache.TryGetValue(cacheKey, out IEnumerable<IEnumerable<ProductListingServiceModel>> cachedProducts))
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
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult<int>> Update([FromForm] UpdateProductServiceModel productModel)
        {
            try
            {
                decimal price;

                if (!decimal.TryParse(productModel.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out price))
                {
                    return BadRequest(new
                    {
                        Key = "Price",
                        Message = "Product price must be a number!"
                    });
                }

                var product = await db.Products
                .FirstOrDefaultAsync(x => x.ProductId == productModel.ProductId);

                if (product == null)
                {
                    return NotFound();
                }

                if (productModel.Quantity < 0)
                {
                    return BadRequest(new
                    {
                        Key = "Quantity",
                        Message = "Quantity must be a positive number!"
                    });
                }

                if (ProductExists(productModel.Name) && product?.Name != productModel.Name)
                {
                    return BadRequest(new
                    {
                        Key = "Name",
                        Message = "Product with this name already exists!"
                    });
                }

                if (price <= 0)
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
                        price,
                        productModel.Quantity,
                        categoriesIds,
                        productImage.ImageData,
                        productImage.ContentType
                    );

                    memoryCache.Remove($"image_{productModel.ProductId}");

                    return productId;
                }
                else
                {
                    var image = await imageService.GetImageByProductId(productModel.ProductId);

                    int productId = await productService
                        .Update(productModel.ProductId,
                        productModel.Name,
                        productModel.Description,
                        price,
                        productModel.Quantity,
                        categoriesIds,
                        image.ImageData,
                        image.ContentType
                    );

                    memoryCache.Remove($"image_{productModel.ProductId}");

                    return productId;
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("{id}")]
        public async Task<ActionResult<bool>> Delete([FromRoute] int id)
        {
            try
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
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("by-category-count")]
        public async Task<ActionResult<bool>> GetByCategoryCount()
        {
            var products = await categoryService.GetProductsCountByCategory();

            return Ok(products);
        }

        [HttpGet]
        [Route("identifiers")]
        public async Task<ActionResult<IEnumerable<int>>> GetProductsIds()
            => Ok(await db.Products
            .Select(x => x.ProductId)
            .OrderBy(x => x)
            .ToListAsync());

        private bool ProductExists(string productName)
        {
            var products = db.Products
                .Select(x => x.Name)
                .ToList();

            if (products.Any(name => name == productName))
            {
                return true;
            }

            return false;
        }
    }
}
