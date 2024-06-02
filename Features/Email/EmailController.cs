namespace NutriBest.Server.Features.Email
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Email.Models;

    public class EmailController : ApiController
    {
        private readonly UserManager<User> userManager;
        private readonly IEmailService emailService;

        public EmailController(UserManager<User> userManager,
            IEmailService emailService)
        {
            this.userManager = userManager;
            this.emailService = emailService;
        }

        [HttpPost]
        public IActionResult SendConfirmOrderEmail([FromBody] EmailConfirmOrderModel request)
        {
            try
            {
                emailService.SendConfirmOrder(request);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route(nameof(ForgottenPassword))]
        public async Task<ActionResult> ForgottenPassword([FromBody] EmailModel request)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(request.To);
                //if (user == null || !await userManager.IsEmailConfirmedAsync(user))
                if (user == null)
                    return Ok(new
                    {
                        IsSuccess = true,
                        Message = "If the email is valid, a password reset link has been sent."
                    });


                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Identity", new { token, email = user.Email }, protocol: HttpContext.Request.Scheme);

                emailService.SendForgottenPassword(request, callbackUrl ?? "");
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
