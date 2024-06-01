namespace NutriBest.Server.Features.Email
{
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.Email.Models;

    public class EmailController : ApiController
    {
        private readonly IEmailService emailService;

        public EmailController(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        [HttpPost]
        public IActionResult SendConfirmOrderEmail([FromBody] EmailConfirmOrderModel request)
        {
            try
            {
                emailService.SendConfirmOrderEmail(request);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
