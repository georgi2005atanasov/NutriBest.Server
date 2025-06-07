using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Admin
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Identity;
    using NutriBest.Server.Features.Profile.Models;
    using NutriBest.Server.Infrastructure.Services;
    using NutriBest.Server.Shared.Responses;
    using static ErrorMessages.ProfileController;

    public class ProfileController : ApiController
    {
        private readonly NutriBestDbContext db;
        private readonly IIdentityService identityService;
        private readonly ICurrentUserService currentUserService;
        private readonly IProfileService profileService;

        public ProfileController(NutriBestDbContext db,
            IIdentityService identityService,
            ICurrentUserService currentUserService,
            IProfileService profileService)
        {
            this.db = db;
            this.identityService = identityService;
            this.currentUserService = currentUserService;
            this.profileService = profileService;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/Profiles")]
        public async Task<ActionResult<AllProfilesServiceModel?>> All([FromQuery] int page,
            [FromQuery] string? search,
            [FromQuery] string? groupType)
        {
            try
            {
                var allProfiles = await profileService.All(page, search, groupType);
                return allProfiles;
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
        [Authorize(Roles = "Administrator,Employee")]
        [Route("{userId}")]
        public async Task<ActionResult<ProfileDetailsServiceModel?>> GetById([FromRoute] string userId)
        {
            try
            {
                var userDetails = await profileService.GetDetailsById(userId);

                return Ok(userDetails);
            }
            catch (ArgumentNullException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.ParamName ?? ""
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
        [Route("Mine")]
        [Authorize(Roles = "Administrator,Employee,User")]
        public async Task<ActionResult<ProfileServiceModel?>> GetCurrentUser()
        {
            try
            {
                var currentUserId = currentUserService.GetUserId();

                if (currentUserId == null)
                    return BadRequest();

                var user = await identityService.FindUserById(currentUserId);
                return user;
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
                return BadRequest(new FailResponse
                {
                    Message = result
                });

            return true;
        }

        [HttpDelete]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<bool>> DeleteProfile()
        {
            try
            {
                var currentUserId = currentUserService.GetUserId();

                var user = await db.Users.FindAsync(currentUserId);
                var profile = await db.Profiles.FindAsync(currentUserId);

                if (user == null)
                    return BadRequest(new FailResponse
                    {
                        Message = UserCouldNotBeFound
                    });

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

        [HttpGet]
        [Route("Address")]
        [Authorize(Roles = "Administrator,Employee,User")]
        public async Task<ActionResult<ProfileAddressServiceModel>> GetAddress()
        {
            try
            {
                var address = await profileService.GetAddress();

                return Ok(address);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("Address")]
        [Authorize(Roles = "Administrator,Employee,User")]
        public async Task<ActionResult<int>> SetAddress([FromBody] ProfileAddressServiceModel addressModel)
        {
            try
            {
                var addressId = await profileService.SetAddress(addressModel.Street,
                    addressModel.StreetNumber,
                    addressModel.City,
                    addressModel.Country,
                    addressModel.PostalCode);

                return Ok(addressId);
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
                return BadRequest();
            }
        }
    }
}
