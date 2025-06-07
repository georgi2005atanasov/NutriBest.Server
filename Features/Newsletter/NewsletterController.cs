namespace NutriBest.Server.Features.Newsletter
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using NutriBest.Server.Features.Newsletter.Models;
    using NutriBest.Server.Shared.Responses;

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

        [HttpDelete]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("Admin/RemoveFromNewsletter")]
        public async Task<ActionResult<bool>> RemoveFromNewsletterByAdmin([FromQuery] string email)
        {
            try
            {
                var result = await newsletterService.RemoveForAdmin(email);

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
        [AllowAnonymous]
        [Route(nameof(Unsubscribe))]
        public async Task<ActionResult<bool>> Unsubscribe([FromQuery] string email,
            [FromQuery] string token)
        {
            try
            {
                var result = await newsletterService
                    .Unsubscribe(email, token);

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
