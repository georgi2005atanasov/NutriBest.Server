namespace NutriBest.Server.Features.Newsletter
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.Newsletter.Models;

    public class NewsletterController : ApiController
    {
        private readonly INewsletterService newsletterService;

        public NewsletterController(INewsletterService newsletterService)
        {
            this.newsletterService = newsletterService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<int>> AddToNewsletter([FromForm] string email)
        {
            try
            {
                var id = await newsletterService.Add(email);

                return Ok(id);
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new
                {
                    err.Message
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult<AllSubscribersServiceModel>> AllSubscribers([FromQuery] int page,
            [FromQuery] string? search = "", [FromQuery] string? type = "all")
        {
            try
            {
                var subscribers = await newsletterService.AllSubscribers(page, search, type);

                return Ok(subscribers);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
