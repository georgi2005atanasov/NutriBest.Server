namespace NutriBest.Server.Features.Admin
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;

    [Authorize(Roles = "Administrator")]
    public class AdminController : ApiController
    {
        private readonly NutriBestDbContext db;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AdminController(NutriBestDbContext db,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpPatch]
        [Route("grant/{id}")]
        public async Task<ActionResult> GrantUser([FromRoute] string id, [FromForm] string role)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id);
                var existingRole = await roleManager.FindByNameAsync(role);

                if (user == null)
                {
                    return BadRequest(new
                    {
                        Message = "User could not be found!"
                    });
                }

                if (existingRole == null)
                {
                    return BadRequest(new
                    {
                        Message = "Invalid role!"
                    });
                }

                if (db.UserRoles.Any(x => x.UserId == user.Id && x.RoleId == existingRole.Id))
                {
                    return BadRequest(new
                    {
                        Message = $"'{user.UserName}' is already in the role of '{role}'!"
                    });
                }

                await userManager.AddToRoleAsync(user, role);

                return Ok(new
                {
                    Message = $"Successfully added role '{role}' to '{user.UserName}'!"
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPatch]
        [Route("disown/{id}")]
        public async Task<ActionResult> DisownUser([FromRoute] string id, [FromForm] string role)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id);
                var existingRole = await roleManager.FindByNameAsync(role);

                if (user == null)
                {
                    return BadRequest(new
                    {
                        Message = "User could not be found!"
                    });
                }

                if (existingRole == null)
                {
                    return BadRequest(new
                    {
                        Message = "Invalid role!"
                    });
                }

                if (!db.UserRoles.Any(x => x.UserId == user.Id && x.RoleId == existingRole.Id))
                {
                    return BadRequest(new
                    {
                        Message = "The user does not have this role!"
                    });
                }

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
    }
}
