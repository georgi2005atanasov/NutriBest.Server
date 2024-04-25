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

        public ProfileController(IIdentityService identityService,
            ICurrentUserService currentUserService,
            NutriBestDbContext db)
        {
            this.identityService = identityService;
            this.currentUserService = currentUserService;
            this.db = db;
        }

        [HttpPut]
        [Authorize(Roles = "Administrator,Employee,User")]
        public async Task<ActionResult<bool>> UpdateProfile(UpdateProfileServiceModel profile)
        {
            var id = currentUserService.GetUserId();

            var user = await db.Users
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest(new
                {
                    Key = "User",
                    Message = "User could not be found"
                });
            }

            if (profile.Age <= 0)
            {
                return BadRequest(new
                {
                    Key = "Age",
                    Message = "Invalid age!"
                });
            }

            if (!string.IsNullOrEmpty(profile.UserName))
            {
                if (user.UserName == profile.UserName)
                {
                    return BadRequest(new
                    {
                        Key = "Username",
                        Message = "Username cannot be the same!"
                    });
                }

                if (await db.Users.AnyAsync(x => x.UserName == profile.UserName))
                {
                    return BadRequest(new
                    {
                        Key = "Username",
                        Message = "This username is already taken!"
                    });
                }
            }

            if (!string.IsNullOrEmpty(profile.Name))
            {
                if (profile.Name == user.Profile.Name)
                {
                    return BadRequest(new
                    {
                        Key = "Name",
                        Message = "Name cannot be the same!"
                    });
                }

                if (await db.Users.AnyAsync(x => x.Profile.Name == profile.Name))
                {
                    return BadRequest(new
                    {
                        Key = "Name",
                        Message = "User with this name already exists!"
                    });
                }
            }

            if (!string.IsNullOrEmpty(profile.Email))
            {
                if (profile.Email == user.Email)
                {
                    return BadRequest(new
                    {
                        Key = "Email",
                        Message = "Email cannot be the same!"
                    });
                }


                if (await db.Users.AnyAsync(x => x.Email == profile.Email))
                {
                    return BadRequest(new
                    {
                        Key = "Email",
                        Message = $"Email '{profile.Email}' is already taken!"
                    });
                }
            }

            if (!string.IsNullOrEmpty(profile.Email))
            {
                user.Email = profile.Email;
            }
            if (!string.IsNullOrEmpty(profile.Name))
            {
                user.Profile.Name = profile.Name;
            }
            if (!string.IsNullOrEmpty(profile.UserName))
            {
                user.UserName = profile.UserName;
            }

            var profileToChange = await db.Profiles
                .FirstAsync(x => x.UserId == id);

            if (!string.IsNullOrEmpty(profile.Name))
            {
                profileToChange.Name = profile.Name;
            }
            if (profile.Age != null)
            {
                profileToChange.Age = profile.Age;
            }

            db.Users.Update(user);
            db.Profiles.Update(profileToChange);

            await db.SaveChangesAsync();

            return true;
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
