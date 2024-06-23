namespace NutriBest.Server.Features.Identity
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Email;
    using NutriBest.Server.Features.Profile.Models;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;
    using NutriBest.Server.Infrastructure.Services;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public class IdentityService : IIdentityService, ITransientService
    {
        private readonly NutriBestDbContext db;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationSettings appSettings;

        public IdentityService(UserManager<User> userManager,
            IOptions<ApplicationSettings> appSettings,
            RoleManager<IdentityRole> roleManager,
            NutriBestDbContext db)
        {
            this.db = db;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.appSettings = appSettings.Value;
        }

        public async Task<List<string>> AllRoles()
        => await roleManager.Roles
                .Select(x => x.Name)
                .Where(x => x != "Administrator")
                .ToListAsync();

        public async Task<bool> CheckUserPassword(User user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IdentityResult> CreateUser(string userName, string email, string password)
        {
            var user = new User
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new Exception(String.Join(", ", result.Errors.Select(x => x.Description)));
            }

            var roleResult = await userManager.AddToRoleAsync(user, "User");

            if (!roleResult.Succeeded)
            {
                throw new Exception("Could not add the role");
            }

            return result;
        }

        public async Task<ProfileServiceModel> FindUserById(string id)
            => await db.Users
                .Where(x => x.Id == id)
                .Select(x => new ProfileServiceModel
                {
                    Gender = x.Profile.Gender.ToString(),
                    Name = x.Profile.Name,
                    Age = x.Profile.Age,
                    ModifiedOn = x.ModifiedOn,
                    CreatedOn = x.CreatedOn,
                    Email = x.Email,
                    UserName = x.UserName
                })
                .FirstAsync();

        public async Task<User> FindUserByUserName(string userName)
        {
            return await userManager.FindByNameAsync(userName);
        }

        public async Task<string> GetEncryptedToken(User user)
        {
            var roles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                        SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encryptedToken = tokenHandler.WriteToken(token);

            return encryptedToken;
        }
    }
}
