using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NutriBest.Server.Features.Products
{
    public class ProductsController : ApiController
    {
        private readonly IProductService productService;

        public ProductsController(IProductService productService)
            => this.productService = productService;

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [Route(nameof(Create))]
        public async Task<ActionResult> Create([FromForm] CreateProductRequestModel productModel)
        {
            if (productModel.Price <= 0)
            {
                return BadRequest("Price must be bigger than zero!");
            }

            if (productModel.Image != null)
            {
                var productImage = await productService
                    .GetImage(productModel.Image, productModel.Image.ContentType);

                var productId = await productService
                    .Create(productModel.Name,
                    productModel.Description,
                    productModel.Price,
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
    }
}
