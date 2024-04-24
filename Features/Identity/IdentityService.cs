namespace NutriBest.Server.Features.Identity
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Identity.Models;
    using NutriBest.Server.Infrastructure.Services;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationSettings appSettings;
        private readonly NutriBestDbContext db;
        private readonly ICurrentUserService currentUser;

        public IdentityService(UserManager<User> userManager,
            IOptions<ApplicationSettings> appSettings,
            NutriBestDbContext db,
            ICurrentUserService currentUser,
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.appSettings = appSettings.Value;
            this.db = db;
            this.currentUser = currentUser;
            this.roleManager = roleManager;
        }

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
            };

            var result = await userManager.CreateAsync(user, password);

            var roleResult = await userManager.AddToRoleAsync(user, "User");

            if (!roleResult.Succeeded)
            {
                throw new Exception("Could not add the role");
            }

            return result;
        }

        public async Task<UserServiceModel> FindUserById(string id)
            => await db.Users
                .Where(x => x.Id == id)
                .Select(x => new UserServiceModel
                {
                    Gender = x.Gender.ToString(),
                    Name = x.Name,
                    ModifiedOn = x.ModifiedOn,
                    CreatedOn = x.CreatedOn
                })
                .FirstOrDefaultAsync();

        public async Task<User> FindUserByName(string userName)
        {
            return await userManager.FindByNameAsync(userName);
        }

        public async Task<string> GetEncryptedToken(User user)
        {
            var roles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
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
