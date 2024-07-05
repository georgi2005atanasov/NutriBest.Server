namespace NutriBest.Server.Features.Images
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Images.Models;

    public class ImagesController : ApiController
    {
        private readonly IImageService imageService;
        private readonly IMemoryCache memoryCache;

        public ImagesController(IImageService imageService, IMemoryCache memoryCache)
        {
            this.imageService = imageService;
            this.memoryCache = memoryCache;
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult<ImageListingServiceModel>>? GetImageByProductId(int id)
        {
            try
            {
                string cacheKey = $"product_image_{id}";
                if (!memoryCache.TryGetValue(cacheKey, out ImageListingServiceModel cachedImage))
                {
                    var image = await imageService.GetImageByProductId(id);

                    if (image == null)
                        return BadRequest();

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(15)) // Sets the time the cache entry can be inactive (not accessed) before it will be removed.
                        .SetAbsoluteExpiration(TimeSpan.FromHours(2)); // Sets a fixed time to live for the cache entry

                    memoryCache.Set(cacheKey, image, cacheEntryOptions);
                    return Ok(image);
                }

                return Ok(cachedImage);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("Brand/{name}")] // important!!!
        public async Task<ActionResult<ImageListingServiceModel>> GetImageByBrandId([FromRoute] string name)
        {
            try
            {
                string cacheKey = $"brand_logo_image_{name}";
                if (!memoryCache.TryGetValue(cacheKey, out ImageListingServiceModel cachedImage))
                {
                    var image = await imageService.GetImageByBrandId(name);

                    if (image == null)
                        return BadRequest();

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(15)) // Sets the time the cache entry can be inactive (not accessed) before it will be removed.
                        .SetAbsoluteExpiration(TimeSpan.FromHours(2)); // Sets a fixed time to live for the cache entry

                    memoryCache.Set(cacheKey, image, cacheEntryOptions);
                    return Ok(image);
                }


                return Ok(cachedImage);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
