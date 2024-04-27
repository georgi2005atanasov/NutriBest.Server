namespace NutriBest.Server.Features.Admin
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Admin.Models;

    public class AdminService : IAdminService
    {
        private readonly NutriBestDbContext db;
        private readonly UserManager<User> userManager;

        public AdminService(NutriBestDbContext db,
            UserManager<User> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        public async Task<IEnumerable<UserServiceModel>> GetAllUsers()
        {
            var users = db.Users
                .Include(x => x.Profile)
                .AsQueryable();

            var usersModels = new List<UserServiceModel>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);

                var userModel = new UserServiceModel
                {
                    IsDeleted = user.IsDeleted,
                    Age = user.Profile.Age,
                    Gender = user.Profile.Gender.ToString(),
                    Name = user.Profile.Name,
                    UserName = user.UserName,
                    Roles = roles
                };

                usersModels.Add(userModel);
            }

            return usersModels;
        }
    }
}
