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

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult<AllSubscribersServiceModel>> AllSubscribers([FromQuery] int page = 1, 
            [FromQuery] string? search = "", 
            [FromQuery] string? groupType = "all")
        {
            try
            {
                var subscribers = await newsletterService.AllSubscribers(page, search, groupType);

                return Ok(subscribers);
            }
            catch(NullReferenceException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
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

        [HttpDelete]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("Admin/RemoveFromNewsletter")]
        public async Task<ActionResult<bool>> RemoveFromNewsletterByAdmin([FromForm] string email)
        {
            try
            {
                var result = await newsletterService.Remove(email);

                return Ok(new
                {
                    Succeeded = result
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("RemoveFromNewsletter")]
        public async Task<ActionResult<bool>> RemoveFromNewsletter([FromForm] string email)
        {
            try
            {
                var result = await newsletterService.Remove(email);

                return Ok(new
                {
                    Succeeded = result
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
