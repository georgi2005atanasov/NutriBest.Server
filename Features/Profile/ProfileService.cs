namespace NutriBest.Server.Features.Admin
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Infrastructure.Services;

    public class ProfileService : IProfileService
    {
        private readonly ICurrentUserService currentUser;
        private readonly NutriBestDbContext db;

        public ProfileService(ICurrentUserService currentUser, NutriBestDbContext db)
        {
            this.currentUser = currentUser;
            this.db = db;
        }

        public async Task<string> UpdateProfile(string? name,
            string? userName,
            string? email,
            int? age)
        {
            var id = currentUser.GetUserId();

            var user = await db.Users
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return "User could not be found";
            }

            if (age <= 0)
            {
                return "Invalid age!";
            }

            if (!string.IsNullOrEmpty(userName))
            {
                if (user.UserName == userName)
                {
                    return "Username cannot be the same!";
                }

                if (await db.Users.AnyAsync(x => x.UserName == userName))
                {
                    return "This username is already taken!";
                }
            }

            if (!string.IsNullOrEmpty(name))
            {
                if (name == user.Profile.Name)
                {
                    return "Name cannot be the same!";
                }

                if (await db.Users.AnyAsync(x => x.Profile.Name == name))
                {
                    return "User with this name already exists!";
                }
            }

            if (!string.IsNullOrEmpty(email))
            {
                if (email == user.Email)
                {
                    return "Email cannot be the same!";

                }


                if (await db.Users.AnyAsync(x => x.Email == email))
                {
                    return $"Email '{email}' is already taken!";
                }
            }

            if (!string.IsNullOrEmpty(email))
            {
                user.Email = email;
            }
            if (!string.IsNullOrEmpty(name))
            {
                user.Profile.Name = name;
            }
            if (!string.IsNullOrEmpty(userName))
            {
                user.UserName = userName;
            }

            var profileToChange = await db.Profiles
                .FirstAsync(x => x.UserId == id);

            if (!string.IsNullOrEmpty(name))
            {
                profileToChange.Name = name;
            }
            if (age != null)
            {
                profileToChange.Age = age;
            }

            db.Users.Update(user);
            db.Profiles.Update(profileToChange);

            await db.SaveChangesAsync();

            return "success";
        }

        public Task DeleteProfile()
        {
            return Task.CompletedTask;
        }
    }
}
