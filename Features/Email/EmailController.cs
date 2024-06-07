namespace NutriBest.Server.Features.Email
{
    using Microsoft.AspNetCore.Authorization;
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
        [Route(nameof(SendConfirmOrderEmail))]
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
            if (string.IsNullOrEmpty(request.To))
                return BadRequest(new
                {
                    Error = "Email is Required!"
                });

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
                //ENSURE WHEN IT GOES TO PRODUCTION TO CHANGE TO HTTPS
                var callbackUrl = Url.Action("ResetPassword", "Identity", new { token, email = user.Email }, protocol: "http", host: "localhost:5173");

                emailService.SendForgottenPassword(request, callbackUrl ?? "");
                return Ok(new
                {
                    IsSuccess = true,
                    Message = "If the email is valid, a password reset link has been sent."
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Employee")]
        [Route(nameof(SendPromoCode))]
        public async Task<ActionResult> SendPromoCode([FromBody] SendPromoEmailModel request)
        {
            if (string.IsNullOrEmpty(request.To))
                return BadRequest(new
                {
                    Error = "Email is Required!"
                });

            try
            {
                await emailService.SendPromoCode(request);

                return Ok(new
                {
                    IsSuccess = true,
                    Message = "Successfully sent promo code!"
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
