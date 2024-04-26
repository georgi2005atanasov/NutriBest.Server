namespace NutriBest.Server.Features.Admin
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Identity;
    using NutriBest.Server.Features.Profile.Models;
    using NutriBest.Server.Infrastructure.Services;

    public class ProfileController : ApiController
    {
        private readonly NutriBestDbContext db;
        private readonly IIdentityService identityService;
        private readonly ICurrentUserService currentUserService;
        private readonly IProfileService profileService;

        public ProfileController(IIdentityService identityService,
            ICurrentUserService currentUserService,
            NutriBestDbContext db,
            IProfileService profileService)
        {
            this.identityService = identityService;
            this.currentUserService = currentUserService;
            this.db = db;
            this.profileService = profileService;
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

        [HttpPut]
        [Authorize(Roles = "Administrator,Employee,User")]
        public async Task<ActionResult<bool>> UpdateProfile([FromForm] UpdateProfileServiceModel profile)
        {
            var result = await profileService.UpdateProfile(profile.Name,
                profile.UserName,
                profile.Email,
                profile.Age,
                profile.Gender);

            if (result != "success")
            {
                return BadRequest(new
                {
                    Message = result
                });
            }

            return true;
        }

        [HttpDelete]
        [Authorize(Roles = "Administrator,Employee,User")]
        public async Task<ActionResult<bool>> DeleteProfile()
        {
            try
            {
                var currentUserId = currentUserService.GetUserId();

                if (currentUserId == null)
                {
                    return BadRequest(new
                    {
                        Message = "Invalid user!"
                    });
                }

                var user = await db.Users.FindAsync(currentUserId);
                var profile = await db.Profiles.FindAsync(currentUserId);

                if (user == null)
                {
                    return BadRequest(new
                    {
                        Message = "User could not be found!"
                    });
                }

                db.Profiles.Remove(profile!);
                db.Users.Remove(user);

                await db.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
