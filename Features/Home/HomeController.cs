using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Home
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using NutriBest.Server.Features.Home.Models;
    using NutriBest.Server.Shared.Responses;
    using static ErrorMessages;

    public class HomeController : ApiController
    {
        private readonly IHomeService homeService;
        private readonly IMemoryCache memoryCache;

        public HomeController(IHomeService homeService,
            IMemoryCache memoryCache)
        {
            this.homeService = homeService;
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        [Route(nameof(ContactUs))]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<ActionResult<ContactUsInfoServiceModel>> ContactUs()
        {
            try
            {
                var contactUsInfo = await homeService.ContactUsDetails();
                return Ok(contactUsInfo);
            }
            catch (Exception)
            {
                return BadRequest(new FailResponse
                {
                    Message = ErrorMessages.Exception
                });
            }
        }
    }
}
