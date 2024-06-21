namespace NutriBest.Server.Features.Admin
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Identity;
    using NutriBest.Server.Features.Profile.Models;
    using NutriBest.Server.Infrastructure.Services;
    using NutriBest.Server.Utilities;
    using System.Text;

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

                Task<ProfileServiceModel> task = identityService.FindUserById(currentUserId);
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

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/Profiles/CSV")]
        public async Task<FileContentResult?> GetCsv([FromQuery] string? search,
            [FromQuery] string? groupType)
        {
            try
            {
                var users = await profileService.All(1, search, groupType); // Assuming this fetches your data
                var csv = ConvertToCsv(users);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "profiles.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string ConvertToCsv(AllProfilesServiceModel users)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Id,Name,Email,MadeOn,IsDeleted,PhoneNumber,City,TotalOrders,TotalSpent");

            foreach (var user in users.Profiles)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(user.ProfileId)},{CsvHelper.EscapeCsvValue(user.Name ?? "-")},{CsvHelper.EscapeCsvValue(user.Email ?? "-")},{CsvHelper.EscapeCsvValue(user.MadeOn.ToString())},{CsvHelper.EscapeCsvValue(user.IsDeleted.ToString())},{CsvHelper.EscapeCsvValue(user.PhoneNumber ?? "-")},{CsvHelper.EscapeCsvValue(user.City ?? "-")},{CsvHelper.EscapeCsvValue(user.TotalOrders.ToString())},{CsvHelper.EscapeCsvValue(user.TotalSpent.ToString())}");
            }

            return csv.ToString();
        }
    }
}
