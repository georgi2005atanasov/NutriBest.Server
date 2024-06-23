namespace NutriBest.Server.Features.Identity
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Email;
    using NutriBest.Server.Features.Identity.Models;
    using NutriBest.Server.Features.Notifications;
    using NutriBest.Server.Infrastructure.Services;

    public class IdentityController : ApiController
    {
        private readonly IIdentityService identityService;
        private readonly UserManager<User> userManager;
        private readonly ICurrentUserService currentUserService;
        private readonly IEmailService emailService;
        private readonly INotificationService notificationService;

        public IdentityController(IIdentityService identityService,
            UserManager<User> userManager,
            ICurrentUserService currentUserService,
            IEmailService emailService,
            INotificationService notificationService)
        {
            this.identityService = identityService;
            this.userManager = userManager;
            this.currentUserService = currentUserService;
            this.emailService = emailService;
            this.notificationService = notificationService;
        }

        [HttpPost]
        [Route(nameof(Register))]
        public async Task<ActionResult> Register(RegisterServiceModel userModel)
        {
            try
            {
                if (userModel.ConfirmPassword != userModel.Password)
                    return BadRequest(new FailResponse
                    {
                        Key = "Password",
                        Message = "Both passwords should match!"
                    });

                if (userModel.UserName.Contains(" "))
                    return BadRequest(new FailResponse
                    {
                        Key = "UserName",
                        Message = "Username must not contain white spaces!"
                    });

                var result = await identityService
                         .CreateUser(userModel.UserName,
                                     userModel.Email,
                                     userModel.Password);

                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(userModel.Email);

                    await notificationService.SendNotificationToAdmin("success", $"'{user.UserName}' Has Just Registered!");

                    return Ok(new SuccessResponse
                    {
                        Message = "Successfully added new user!"
                    });
                }

                return BadRequest(result.Errors);
            }
            catch (Exception err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.Message
                });
            }
        }

        [HttpPost]
        [Route(nameof(Login))]
        public async Task<ActionResult<string>> Login(LoginServiceModel userModel)
        {
            try
            {
                var user = await identityService.FindUserByUserName(userModel.UserName);

                if (user == null)
                    return Unauthorized();

                if (user.IsDeleted)
                    return Unauthorized();

                var passwordValid = await identityService.CheckUserPassword(user, userModel.Password);

                if (!passwordValid)
                    return Unauthorized();

                var encryptedToken = await identityService.GetEncryptedToken(user);

                await notificationService.SendNotificationToAdmin("success", $"'{user.UserName}' Has Just Logged In!");

                return encryptedToken;
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [Route(nameof(ResetPassword))]
        public async Task<ActionResult<bool>> ResetPassword([FromBody] ResetPasswordServiceModel resetModel)
        {
            try
            {
                if (resetModel.NewPassword != resetModel.ConfirmPassword)
                    return BadRequest(new FailResponse
                    {
                        Key = "NewPassword",
                        Message = "Both passwords should match!"
                    });

                var user = await userManager.FindByEmailAsync(resetModel.Email);
                if (user == null)
                    return Ok(new SuccessResponse
                    {
                        Message = "Password reset successful."
                    });

                var result = await userManager.ResetPasswordAsync(user, resetModel.Token, resetModel.NewPassword);
                if (result.Succeeded)
                    return Ok(new SuccessResponse
                    {
                        Message = "Password reset successful."
                    });

                return BadRequest(new FailResponse
                { 
                    Message = "Error resetting password." 
                });
            }
            catch (Exception)
            {
                return BadRequest(new FailResponse
                {
                    Message = "Something went wrong!"
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("Roles")]
        public async Task<ActionResult<List<string>>> AllRoles()
        {
            try
            {
                var roles = await identityService.AllRoles();

                return Ok(new
                {
                    Roles = roles
                });
            }
            catch (Exception)
            {
                return BadRequest(new FailResponse
                {
                    Message = "Error occured when fetching the roles!"
                });
            }
        }
    }
}