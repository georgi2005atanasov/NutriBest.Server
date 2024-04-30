namespace NutriBest.Server.Features.Admin
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Enums;
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
            int? age,
            string? gender)
        {
            var id = currentUser.GetUserId();

            var user = await db.Users
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            var profile = await db.Profiles
                .Where(x => x.UserId == id)
                .FirstOrDefaultAsync();

            if (user == null || profile == null)
                return "User could not be found";

            if (age <= 0)
                return "Invalid age!";

            if (!string.IsNullOrEmpty(userName))
            {
                if (user.UserName == userName)
                    return "Username cannot be the same!";

                if (await db.Users.AnyAsync(x => x.UserName == userName))
                    return "This username is already taken!";
            }

            if (!string.IsNullOrEmpty(name))
            {
                if (name == profile.Name)
                    return "Name cannot be the same!";

                if (await db.Users.AnyAsync(x => profile.Name == name))
                    return "User with this name already exists!";
            }

            if (!string.IsNullOrEmpty(email))
            {
                if (email == user.Email)
                    return "Email cannot be the same!";

                if (await db.Users.AnyAsync(x => x.Email == email))
                    return $"Email '{email}' is already taken!";
            }

            if (age != null && age == profile.Age)
                return $"Age must be different than the previous";

            Gender genderRes = Gender.Unspecified;

            if (!string.IsNullOrEmpty(gender) && !Enum.TryParse<Gender>(gender, true, out genderRes))
                return $"{gender} is invalid Gender!";

            if (gender == profile.Gender.ToString())
                return "The gender must be different from the previous!";

            if (!string.IsNullOrEmpty(email))
                user.Email = email;

            if (!string.IsNullOrEmpty(name))
                profile.Name = name;

            if (!string.IsNullOrEmpty(userName))
                user.UserName = userName;

            if (age != null)
                profile.Age = age;

            if (!string.IsNullOrEmpty(gender))
                profile.Gender = genderRes;

            db.Users.Update(user);
            db.Profiles.Update(profile);

            await db.SaveChangesAsync();

            return "success";
        }
    }
}
