namespace NutriBest.Server.Features.Newsletter
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
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

    }
}
