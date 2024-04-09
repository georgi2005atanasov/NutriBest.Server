using Microsoft.AspNetCore.Mvc;

namespace NutriBest.Server.Features.Identity
{
    public class IdentityController : ApiController
    {
        private readonly IIdentityService identityService;
        public IdentityController(
            IIdentityService identityService)
        {
            this.identityService = identityService;
        }

        [Route(nameof(Register))]
        [HttpPost]
        public async Task<ActionResult> Register(RegisterRequestModel userModel)
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

        [Route(nameof(Login))]
        [HttpPost]
        public async Task<ActionResult<string>> Login(LoginRequestModel userModel)
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
    }
}
