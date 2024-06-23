namespace NutriBest.Server.Features.Home
{
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.Home.Models;

    public class HomeController : ApiController
    {
        private readonly IHomeService homeService;

        public HomeController(IHomeService homeService)
        {
            this.homeService = homeService;
        }

        [HttpGet]
        [Route(nameof(ContactUs))]
        public async Task<ActionResult<ContactUsInfoServiceModel>> ContactUs()
        {
            try
            {
                var contactUsInfo = await homeService.ContactUsDetails();

                return Ok(contactUsInfo);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
