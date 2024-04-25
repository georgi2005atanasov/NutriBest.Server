namespace NutriBest.Server.Features.Admin
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.Identity;
    using NutriBest.Server.Features.Profile.Models;
    using NutriBest.Server.Infrastructure.Services;

    public class ProfileController : ApiController
    {
        private readonly IIdentityService identityService;
        private readonly ICurrentUserService currentUserService;

        public ProfileController(IIdentityService identityService,
            ICurrentUserService currentUserService)
        {
            this.identityService = identityService;
            this.currentUserService = currentUserService;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("{userId}")]
        public async Task<ActionResult<ProfileServiceModel?>> GetById(string userId)
        {
            try
            {
                Task<ProfileServiceModel>? task = identityService.FindUserById(userId);
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
        public async Task<ActionResult<ProfileServiceModel?>> GetCurrentUser()
        {
            try
            {
                var currentUserId = currentUserService.GetUserId();

                if (currentUserId == null)
                {
                    return BadRequest();
                }

                Task<ProfileServiceModel>? task = identityService.FindUserById(currentUserId);
                return await task!;
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
