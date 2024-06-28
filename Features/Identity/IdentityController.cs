using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Identity
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Authorization;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Email;
    using NutriBest.Server.Shared.Responses;
    using NutriBest.Server.Features.Notifications;
    using NutriBest.Server.Infrastructure.Services;
    using NutriBest.Server.Features.Identity.Models;
    using static ErrorMessages.IdentityController;
    using static SuccessMessages.IdentityController;

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
                        Message = BothPasswordsShouldMatch
                    });

                if (userModel.UserName.Contains(" "))
                    return BadRequest(new FailResponse
                    {
                        Key = "UserName",
                        Message = UserNameWithWhiteSpaces
                    });

                var result = await identityService
                         .CreateUser(userModel.UserName,
                                     userModel.Email,
                                     userModel.Password);

                if (result.Succeeded)
                {
                    var user = await userManager
                                    .FindByEmailAsync(userModel.Email);

                    await notificationService
                        .SendNotificationToAdmin("success", string.Format(UserHasJustRegistered, user.UserName));

                    return Ok(new SuccessResponse
                    {
                        Message = SuccessfullyAddedUser
                    });
                }

                return BadRequest(result.Errors);
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
                return BadRequest(new FailResponse
                {
                    Message = ErrorMessages.Exception
                });
            }
        }

        [HttpPost]
        [Route(nameof(Login))]
        public async Task<ActionResult<string>> Login(LoginServiceModel userModel)
        {
            try
            {
                var user = await identityService
                    .FindUserByUserName(userModel.UserName);

                if (user == null)
                    return Unauthorized();

                if (user.IsDeleted)
                    return Unauthorized();

                var passwordValid = await identityService
                    .CheckUserPassword(user, userModel.Password);

                if (!passwordValid)
                    return Unauthorized();

                var encryptedToken = await identityService
                    .GetEncryptedToken(user);

                await notificationService
                    .SendNotificationToAdmin("success", string.Format(UserHasJustLoggedIn, user.UserName));

                return encryptedToken;
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
                return BadRequest(new FailResponse
                {
                    Message = ErrorMessages.Exception
                });
            }
        }

        [HttpPut]
        [Route(nameof(ResetPassword))]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordServiceModel resetModel)
        {
            try
            {
                if (resetModel.NewPassword != resetModel.ConfirmPassword)
                    return BadRequest(new FailResponse
                    {
                        Key = "NewPassword",
                        Message = BothPasswordsShouldMatch
                    });

                var user = await userManager
                                .FindByEmailAsync(resetModel.Email);

                if (user == null)
                    return Ok(new SuccessResponse
                    {
                        Message = PasswordResetSuccessful
                    });

                var result = await userManager
                    .ResetPasswordAsync(user,
                                        resetModel.Token,
                                        resetModel.NewPassword);

                if (result.Succeeded)
                    return Ok(new SuccessResponse
                    {
                        Message = PasswordResetSuccessful
                    });

                return BadRequest(new FailResponse
                {
                    Message = ErrorResetingPassword
                });
            }
            catch (Exception)
            {
                return BadRequest(new FailResponse
                {
                    Message = ErrorMessages.Exception
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route(nameof(Roles))]
        public async Task<ActionResult<RolesServiceModel>> Roles()
        {
            try
            {
                var roles = await identityService
                            .AllRoles();

                return Ok(new RolesServiceModel
                {
                    Roles = roles
                });
            }
            catch (Exception)
            {
                return NotFound(new FailResponse
                {
                    Message = ErrorWhenFetchingRoles
                });
            }
        }
    }
}