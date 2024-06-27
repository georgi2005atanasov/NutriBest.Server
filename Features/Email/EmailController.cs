namespace NutriBest.Server.Features.Email
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Authorization;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Email.Models;

    public class EmailController : ApiController
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration config;
        private readonly IEmailService emailService;

        public EmailController(UserManager<User> userManager,

            IEmailService emailService,
            IConfiguration config)
        {
            this.userManager = userManager;
            this.config = config;
            this.emailService = emailService;
        }

        [HttpPost]
        [Route(nameof(SendConfirmOrderEmail))]
        public async Task<IActionResult> SendConfirmOrderEmail([FromBody] EmailConfirmOrderModel request)
        {
            try
            {
                await emailService.SendConfirmOrder(request);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route(nameof(SendConfirmedOrderToAdmin))]
        public async Task<IActionResult> SendConfirmedOrderToAdmin([FromBody] EmailConfirmedOrderModel confirmedOrderModel)
        {
            try
            {
                await emailService.SendConfirmedOrderToAdmin(confirmedOrderModel);

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route(nameof(SendOrderToAdmin))]
        public ActionResult SendOrderToAdmin([FromBody] EmailOrderModel request)
        {
            try
            {
                emailService.SendNewOrderToAdmin(request);
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
                if (user == null)
                    return Ok(new
                    {
                        IsSuccess = true,
                        Message = "If the email is valid, a password reset link has been sent."
                    });

                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                //ENSURE WHEN IT GOES TO PRODUCTION TO CHANGE TO HTTPS
                var host = config.GetSection("ClientHost").Value;
                var callbackUrl = Url.Action("ResetPassword", "Identity", new { token, email = user.Email }, protocol: "http", host: host);

                await emailService.SendForgottenPassword(request, callbackUrl ?? "");
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

        [HttpPost]
        [Route(nameof(SendJoinedToNewsletter))]
        public async Task<ActionResult> SendJoinedToNewsletter([FromBody] EmailModel request)
        {
            if (string.IsNullOrEmpty(request.To))
                return BadRequest(new
                {
                    Error = "Email is Required!"
                });

            try
            {
                await emailService.SendJoinedToNewsletter(request);

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

        [HttpPost]
        [Route(nameof(SendMessageToSubscribers))]
        public async Task<ActionResult> SendMessageToSubscribers([FromBody] EmailSubscribersServiceModel request,
            [FromQuery] string groupType)
        {
            try
            {
                await emailService.SendMessageToSubscribers(request, groupType);

                return Ok(new
                {
                    IsSuccess = true,
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route(nameof(SendPromoCodesToSubscribers))]
        public async Task<ActionResult> SendPromoCodesToSubscribers([FromBody] EmailSubscribersPromoCodeServiceModel request,
            [FromQuery] string groupType)
        {
            try
            {
                await emailService.SendPromoCodesToSubscribers(request, groupType);

                return Ok(new
                {
                    IsSuccess = true,
                });
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
                return BadRequest(new
                {
                    IsSuccess = false
                });
            }
        }
    }
}
