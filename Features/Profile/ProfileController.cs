namespace NutriBest.Server.Features.Admin
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
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
                return BadRequest(new
                {
                    Message = "User could not be found!"
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("{userId}")]
        public async Task<ActionResult<ProfileServiceModel?>> GetById([FromRoute] string userId)
        {
            try
            {
                var userDetails = await profileService.GetDetailsById(userId);

                return Ok(userDetails);
            }
            catch (ArgumentNullException err)
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
                    Message = "User could not be found!"
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
                return BadRequest(new
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

                if (currentUserId == null)
                    return BadRequest(new
                    {
                        Message = "Invalid user!"
                    });

                var user = await db.Users.FindAsync(currentUserId);
                var profile = await db.Profiles.FindAsync(currentUserId);

                if (user == null)
                    return BadRequest(new
                    {
                        Message = "User could not be found!"
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
        public async Task<ActionResult<ProfileAddressServiceModel>> SetAddress([FromBody] ProfileAddressServiceModel addressModel)
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
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
