namespace NutriBest.Server.Features.Identity
{
    using Microsoft.AspNetCore.Authorization;
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

        [Route(nameof(Register))]
        [HttpPost]
        public async Task<ActionResult> Register(RegisterRequestModel userModel)
        {
            try
            {
                var result = await identityService
                         .CreateUser(userModel.UserName,
                                     userModel.Email,
                                     userModel.Password);

                if (result.Succeeded)
                {
                    return Ok("Successfully added new user!");
                }

                return BadRequest(result.Errors);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route(nameof(Login))]
        [HttpPost]
        public async Task<ActionResult<string>> Login(LoginRequestModel userModel)
        {
            try
            {
                var user = await identityService.FindUserByName(userModel.UserName);

                if (user == null)
                {
                    return Unauthorized();
                }

                var passwordValid = await identityService.CheckUserPassword(user, userModel.Password);

                if (!passwordValid)
                {
                    return Unauthorized();
                }

                var encryptedToken = await identityService.GetEncryptedToken(user);
                return encryptedToken;
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("{userId}")]
        public async Task<ActionResult<UserServiceModel?>> GetById(string userId)
        {
            try
            {
                Task<UserServiceModel>? task = identityService.FindUserById(userId);
                return await task!;
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("mine")]
        [Authorize(Roles = "Administrator,Employee,User")]
        public async Task<ActionResult<UserServiceModel?>> GetCurrentUser()
        {
            try
            {
                var currentUserId = currentUserService.GetUserId();

                if (currentUserId == null)
                {
                    return BadRequest();
                }

                Task<UserServiceModel>? task = identityService.FindUserById(currentUserId);
                return await task!;
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

    }
}
