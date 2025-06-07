using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Products
{
    using System.Text.Json;
    using System.Globalization;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Categories;
    using NutriBest.Server.Features.Images;
    using NutriBest.Server.Shared.Responses;
    using NutriBest.Server.Features.Products.Models;
    using NutriBest.Server.Features.Categories.Models;
    using static ErrorMessages.ProductsController;

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
        // Changed From ProductListingServiceModel
        public async Task<ActionResult<ProductServiceModel>> Get([FromRoute] int id)
        {
            try
            {
                var product = await productService.Get(id);

                return Ok(product);
            }
            catch (ArgumentNullException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.ParamName ?? ""
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("Specs/{id}/{name}")]
        public async Task<ActionResult<List<ProductSpecsServiceModel>>> GetSpecs([FromRoute] int id,
            [FromRoute] string name)
        {
            try
            {
                var specs = await productService.GetSpecs(id, name);

                return Ok(specs);
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.Message
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult> Create([FromForm] CreateProductRequestModel productModel)
        {
            try
            {
                var productSpecs = JsonSerializer.Deserialize<List<ProductSpecsServiceModel>>(productModel.ProductSpecs)
                    ?? new List<ProductSpecsServiceModel>();

                if (productSpecs.Count == 0)
                    return BadRequest(new FailResponse
                    {
                        Key = "ProductSpecs",
                        Message = ProductsSpecificationAreRequired
                    });

                var passedCategories = productModel.Categories[0]
                .Split(",")
                .ToList();

                if (productModel.Image == null)
                    return BadRequest(new FailResponse
                    {
                        Key = "Image",
                        Message = ImageIsRequired
                    });

                if (ProductExists(productModel.Name))
                    return BadRequest(new FailResponse
                    {
                        Key = "Name",
                        Message = ProductAlreadyExists
                    });

                foreach (var productSpec in productSpecs)
                {
                    if (!decimal.TryParse(productModel.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
                    {
                        return BadRequest(new FailResponse
                        {
                            Key = "Price",
                            Message = PricesMustBeNumbers
                        });
                    }


                    if (price <= 0 || price > 4000)
                        return BadRequest(new FailResponse
                        {
                            Key = "ProductSpecs",
                            Message = InvalidPriceRange
                        });
                }

                var categoriesIds = await categoryService
                    .GetCategoriesIds(passedCategories);

                if (categoriesIds.Count == 0)
                {
                    return BadRequest(new FailResponse
                    {
                        Key = "Category",
                        Message = AtLeastOneCategory
                    });
                }

                var productImage = await imageService
                    .CreateImage<ProductImage>(productModel.Image, productModel.Image.ContentType);

                var productId = await productService
                    .Create(productModel.Name,
                    productModel.Description,
                    productModel.Brand,
                    categoriesIds,
                    productSpecs,
                    productImage.ImageData,
                    productImage.ContentType
                    );

                return Created(nameof(Create), productId);
            }
            catch (ArgumentNullException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.ParamName ?? ""
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<ActionResult<AllProductsServiceModel>> All(
            [FromQuery] int page = 1,
            [FromQuery] string? categories = "",
            [FromQuery] string? brand = "",
            [FromQuery] string? price = "",
            [FromQuery] string? alpha = "",
            [FromQuery] string? productsView = "all",
            [FromQuery] string? search = "",
            [FromQuery] string? priceRange = "",
            [FromQuery] string? quantities = "",
            [FromQuery] string? flavours = "") //might add from the query filters
        {
            try
            {
                if (page < 1)
                    return BadRequest();

                if (productsView == "table" &&
                    !User.IsInRole("Administrator") && !User.IsInRole("Employee"))
                    return BadRequest();

                var products = await productService.All(page,
                    categories,
                    brand,
                    price,
                    alpha,
                    productsView,
                    search,
                    priceRange,
                    quantities,
                    flavours);

                return Ok(products);
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.Message
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("{id}")]
        public async Task<ActionResult<int>> Update([FromRoute] int id, [FromForm] UpdateProductServiceModel productModel)
        {
            try
            {
                var product = await db.Products
                .FirstOrDefaultAsync(x => x.ProductId == id);

                if (product == null)
                    return NotFound();

                var productSpecs = JsonSerializer.Deserialize<List<ProductSpecsServiceModel>>(productModel.ProductSpecs)
                    ?? new List<ProductSpecsServiceModel>();

                if (productSpecs.Count == 0)
                    return BadRequest(new FailResponse
                    {
                        Key = "ProductSpecs",
                        Message = ProductsSpecificationAreRequired
                    });

                foreach (var productSpec in productSpecs)
                {
                    string currentPrice = productSpec.Price.Replace(',', '.');
                    if (!decimal.TryParse(currentPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                        return BadRequest(new FailResponse
                        {
                            Key = "Price",
                            Message = PricesMustBeNumbers
                        });

                    if (price <= 0 || price > 4000)
                        return BadRequest(new FailResponse
                        {
                            Key = "ProductSpecs",
                            Message = InvalidPriceRange
                        });
                }

                if (ProductExists(productModel.Name) && product?.Name != productModel.Name)
                    return BadRequest(new
                    {
                        Key = "Name",
                        Message = ProductAlreadyExists
                    });

                var categoriesIds = await categoryService
                    .GetCategoriesIds(productModel.Categories[0].Split(",").ToList());

                if (categoriesIds.Count == 0)
                    BadRequest(new
                    {
                        Key = "Category",
                        Message = AtLeastOneCategory
                    });

                if (product.PromotionId != null)
                {
                    var promotion = await db.Promotions
                        .FirstAsync(x => x.PromotionId == product.PromotionId);

                    if (promotion.DiscountAmount != null && decimal.Parse(productSpecs.OrderBy(x => x.Price).First().Price) <= promotion.DiscountAmount)
                    {
                        return BadRequest(new
                        {
                            Key = "Price",
                            Message = MustHaveBiggerPriceBecauseOfTheAppliedPromotion
                        });
                    }
                }

                if (productModel.Image != null)
                {
                    var productImage = await imageService
                        .CreateImage<ProductImage>(productModel.Image, productModel.Image.ContentType);

                    int productId = await productService
                        .Update(id,
                        productModel.Name,
                        productModel.Description,
                        productModel.Brand,
                        categoriesIds,
                        productSpecs,
                        productImage.ImageData,
                        productImage.ContentType
                    );

                    memoryCache.Remove($"image_{id}");

                    return productId;
                }
                else
                {
                    var image = await imageService.GetImageByProductId(id);

                    int productId = await productService
                        .Update(id,
                        productModel.Name,
                        productModel.Description,
                        productModel.Brand,
                        categoriesIds,
                        productSpecs,
                        image.ImageData,
                        image.ContentType
                    );

                    memoryCache.Remove($"image_{id}");

                    return productId;
                }
            }
            catch (ArgumentNullException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.ParamName ?? ""
                });
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
                    return NotFound();

                var result = await productService.Delete(id);

                memoryCache.Remove($"product_image_{id}");

                return result;
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult<int>> PatchProduct([FromRoute] int id, [FromForm] PartialUpdateProductServiceModel productModel)
        {
            try
            {
                var result = await productService.PartialUpdate(id, productModel.Description);

                return Ok(result);
            }
            catch (ArgumentNullException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.ParamName ?? ""
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("ByCategoryCount")]
        public async Task<ActionResult<IEnumerable<CategoryCountServiceModel>>> GetByCategoryCount()
        {
            var products = await categoryService.GetProductsCountByCategory();
            return Ok(products);
        }

        [HttpGet]
        [Route("Identifiers")]
        public async Task<ActionResult<List<int>>> GetProductsIds()
            => Ok(await db.Products
            .Select(x => x.ProductId)
            .OrderBy(x => x)
            .ToListAsync());

        [HttpGet]
        [Route("{promotionId}/{productId}")]
        public async Task<ActionResult<ProductWithPromotionDetailsServiceModel>> GetWithPromotion([FromRoute] int promotionId,
            [FromRoute] int productId)
        {
            try
            {
                var product = await productService.GetWithPromotion(productId, promotionId);

                return Ok(product);
            }
            catch (ArgumentNullException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.ParamName ?? ""
                });
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new FailResponse
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
        [Route("Related")]
        public async Task<ActionResult<List<ProductListingServiceModel>>> GetRelatedProductsByCategory([FromBody] RelatedProductsServiceModel productModel)
        {
            try
            {
                var products = await productService.GetRelatedProducts(productModel.Categories, productModel.ProductId);

                return Ok(products);
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new FailResponse
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
        [Route("CurrentPriceWithQuantity")]
        public async Task<ActionResult<ProductPriceQuantityServiceModel>> GetCurrentPriceWithQuantity([FromBody] CurrentProductPriceServiceModel productModel)
        {
            try
            {
                var product = await productService
                    .GetCurrentPriceWithQuantity(productModel.ProductId,
                                                 productModel.Flavour,
                                                 productModel.Package);

                return Ok(product);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        private bool ProductExists(string productName)
        {
            var products = db.Products
                .Select(x => x.Name)
                .ToList();

            if (products.Any(name => name == productName))
                return true;

            return false;
        }
    }
}
