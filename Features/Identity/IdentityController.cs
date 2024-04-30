namespace NutriBest.Server.Features.Identity
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Identity.Models;
    using NutriBest.Server.Infrastructure.Services;

    public class IdentityController : ApiController
    {
        private readonly IIdentityService identityService;
        private readonly UserManager<User> userManager;
        private readonly ICurrentUserService currentUserService;

        public IdentityController(IIdentityService identityService,
            UserManager<User> userManager,
            ICurrentUserService currentUserService)
        {
            this.identityService = identityService;
            this.userManager = userManager;
            this.currentUserService = currentUserService;
        }

        [HttpPost]
        [Route(nameof(Register))]
        public async Task<ActionResult> Register(RegisterServiceModel userModel)
        {
            try
            {
                if (userModel.ConfirmPassword != userModel.Password)
                    return BadRequest(new {
                        Key = "Password",
                        Message = "Both passwords should match!"
                    });

                var result = await identityService
                         .CreateUser(userModel.UserName,
                                     userModel.Email,
                                     userModel.Password);

                if (result.Succeeded)
                    return Ok(new
                    {
                        Message = "Successfully added new user!"
                    });

                return BadRequest(result.Errors);
            }
            catch (Exception err)
            {
                return BadRequest(new
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
                return encryptedToken;
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
