using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Admin
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Authorization;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Shared.Responses;
    using static ErrorMessages.AdminController;
    using static SuccessMessages.AdminController;

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

        [HttpPatch]
        [Route("Grant/{id}")]
        public async Task<ActionResult> GrantUser([FromRoute] string id, [FromQuery] string role)
        {
            try
            {
                var (user, existingRole) = await CheckUserAndRole(id, role);

                if (db.UserRoles.Any(x => x.UserId == user.Id && x.RoleId == existingRole.Id))
                    return BadRequest(new FailResponse
                    {
                        Message = string.Format(UserIsAlreadyInThisRole, user.UserName, role)
                    });

                await userManager.AddToRoleAsync(user, role);

                return Ok(new SuccessResponse
                {
                    Message = string.Format(SuccessfullyAddedRole, role, user.UserName)
                });
            }
            catch (ArgumentNullException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.ParamName!
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

        [HttpPatch]
        [Route("Disown/{id}")]
        public async Task<ActionResult> DisownUser([FromRoute] string id, [FromQuery] string role)
        {
            try
            {
                var (user, existingRole) = await CheckUserAndRole(id, role);

                if (!db.UserRoles.Any(x => x.UserId == user.Id && x.RoleId == existingRole.Id))
                    return BadRequest(new FailResponse
                    {
                        Message = UserDoesNotHaveThisRole
                    });

                await userManager.RemoveFromRoleAsync(user, role);

                return Ok(new FailResponse
                {
                    Message = string.Format(SuccessfullyRemovedRole, role, user.UserName)
                });
            }
            catch (ArgumentNullException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.ParamName!
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

        [HttpPost]
        [Route("Restore/{userId}")]
        public async Task<ActionResult> Restore([FromRoute] string userId)
        {
            try
            {
                var restoredProfileEmail = await adminService.RestoreUser(userId);

                return Ok(new FailResponse
                {
                    Message = string.Format(SuccessfullyRestoredProfile, restoredProfileEmail)
                });
            }
            catch (ArgumentNullException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.ParamName!
                });
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    Message = ErrorMessages.UserNotFound
                });
            }
        }

        [HttpDelete]
        [Route("DeleteUser/{id}")]
        public async Task<ActionResult<bool>> DeleteUser([FromRoute] string id)
        {
            try
            {
                var result = await adminService.DeleteUser(id);
                return Ok(result);
            }
            catch (ArgumentNullException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.ParamName!
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

        private async Task<(User user, IdentityRole existingRole)> CheckUserAndRole(string id, string role)
        {
            var user = await userManager
                            .FindByIdAsync(id);
            var existingRole = await roleManager
                                    .FindByNameAsync(role);

            if (user == null)
                throw new ArgumentNullException(ErrorMessages.UserNotFound);

            if (existingRole == null)
                throw new ArgumentNullException(InvalidRole);

            return (user, existingRole);
        }
    }
}
