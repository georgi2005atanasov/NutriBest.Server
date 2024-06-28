using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Admin
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;
    using static ErrorMessages.AdminController;

    public class AdminService : IAdminService, ITransientService
    {
        private readonly NutriBestDbContext db;

        public AdminService(NutriBestDbContext db)
            => this.db = db;

        public async Task<bool> DeleteUser(string id)
        {
            var user = await db.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            var profile = await db.Profiles
               .FirstOrDefaultAsync(x => x.UserId == id);

            if (profile == null || user == null)
                throw new ArgumentNullException(InvalidUser);

            db.Profiles
                .Remove(profile);

            db.Users
                .Remove(user);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<string> RestoreUser(string id)
        {
            var user = await db.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            var profile = await db.Profiles
               .FirstOrDefaultAsync(x => x.UserId == id);

            if (profile == null || user == null)
                throw new ArgumentNullException(InvalidUser);

            user.IsDeleted = false;
            profile.IsDeleted = false;

            await db.SaveChangesAsync();

            return user.Email;
        }
    }
}
