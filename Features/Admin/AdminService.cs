namespace NutriBest.Server.Features.Admin
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Admin.Models;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;

    public class AdminService : IAdminService, ITransientService
    {
        private readonly NutriBestDbContext db;
        private readonly UserManager<User> userManager;

        public AdminService(NutriBestDbContext db,
            UserManager<User> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        public async Task<bool> DeleteProfile(string id)
        {
            var user = await db.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            var profile = await db.Profiles
               .FirstOrDefaultAsync(x => x.UserId == id);

            if (profile == null || user == null)
                throw new ArgumentNullException("Invalid user!");

            db.Profiles.Remove(profile);
            db.Users.Remove(user);

            await db.SaveChangesAsync();

            return true;
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

        public async Task<string> RestoreProfile(string id)
        {
            var user = await db.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            var profile = await db.Profiles
               .FirstOrDefaultAsync(x => x.UserId == id);

            if (profile == null || user == null)
                throw new ArgumentNullException("Invalid user!");

            user.IsDeleted = false;
            profile.IsDeleted = false;

            await db.SaveChangesAsync();

            return user.Email;
        }
    }
}
