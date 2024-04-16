namespace NutriBest.Server.Features.Images
{
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Data.Models;

    public class ImagesController : ApiController
    {
        private readonly IImageService imageService;

        public ImagesController(IImageService imageService)
            => this.imageService = imageService;

        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult<ProductImage>>? GetImageByProductId(int id)
        {
            var image = await imageService.GetImageByProductId(id);

            if (image == null)
            {
                return BadRequest();
            }

            return Ok(image);
        }
    }
}
