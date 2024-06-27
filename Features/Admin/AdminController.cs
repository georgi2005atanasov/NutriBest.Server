namespace NutriBest.Server.Features.Admin
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Authorization;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Shared.Responses;
    using NutriBest.Server.Features.Admin.Models;

    [Authorize(Roles = "Administrator")]
    public class AdminController : ApiController
    {
        private readonly NutriBestDbContext db;
        private readonly IAdminService adminService;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AdminController(NutriBestDbContext db,
            IAdminService adminService,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.adminService = adminService;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        [Route(nameof(AllUsers))]
        public async Task<ActionResult<IEnumerable<UserServiceModel>>> AllUsers()
        {
            var users = await adminService.GetAllUsers();

            return Ok(users);
        }

        [HttpPatch]
        [Route("Grant/{id}")]
        public async Task<ActionResult> GrantUser([FromRoute] string id, [FromQuery] string role)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id);
                var existingRole = await roleManager.FindByNameAsync(role);

                if (user == null)
                    return BadRequest(new FailResponse
                    {
                        Message = "User could not be found!"
                    });

                if (existingRole == null)
                    return BadRequest(new FailResponse
                    {
                        Message = "Invalid role!"
                    });

                if (db.UserRoles.Any(x => x.UserId == user.Id && x.RoleId == existingRole.Id))
                    return BadRequest(new FailResponse
                    {
                        Message = $"'{user.UserName}' is already in the role of '{role}'!"
                    });

                await userManager.AddToRoleAsync(user, role);

                return Ok(new SuccessResponse
                {
                    Message = $"Successfully added role '{role}' to '{user.UserName}'!"
                });
            }
            catch (Exception)
            {
                return BadRequest(new FailResponse
                {
                    Message = "Something went wrong!"
                });
            }
        }

        [HttpPatch]
        [Route("Disown/{id}")]
        public async Task<ActionResult> DisownUser([FromRoute] string id, [FromQuery] string role)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id);
                var existingRole = await roleManager.FindByNameAsync(role);

                if (user == null)
                    return BadRequest(new
                    {
                        Message = "User could not be found!"
                    });

                if (existingRole == null)
                    return BadRequest(new
                    {
                        Message = "Invalid role!"
                    });

                if (!db.UserRoles.Any(x => x.UserId == user.Id && x.RoleId == existingRole.Id))
                    return BadRequest(new
                    {
                        Message = "The user does not have this role!"
                    });

                await userManager.RemoveFromRoleAsync(user, role);

                return Ok(new
                {
                    Message = $"Successfully removed role '{role}' from '{user.UserName}'!"
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("Restore/{userId}")]
        public async Task<ActionResult> RestoreProfile([FromRoute] string userId)
        {
            try
            {
                var restoredProfileEmail = await adminService.RestoreProfile(userId);

                return Ok(new
                {
                    Message = $"Successfully restored profile with email '{restoredProfileEmail}'!"
                });
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
                    Message = "User not Found!"
                });
            }
        }

        [HttpDelete]
        [Route("DeleteProfile/{id}")]
        public async Task<ActionResult<bool>> DeleteProfile([FromRoute] string id) 
        {
            try
            {
                var result = await adminService.DeleteProfile(id);
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
