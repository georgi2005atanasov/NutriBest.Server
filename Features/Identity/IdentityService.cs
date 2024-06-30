using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Identity
{
    using System.Text;
    using System.Security.Claims;
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Extensions.Options;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Profile.Models;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;
    using static ErrorMessages.AdminController;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    public class IdentityService : IIdentityService, ITransientService
    {
        private readonly NutriBestDbContext db;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationSettings appSettings;
        private readonly IMapper? mapper;

        public IdentityService(NutriBestDbContext db,
            UserManager<User> userManager,
            IOptions<ApplicationSettings> appSettings,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper)
        {
            this.db = db;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.appSettings = appSettings.Value;
            this.mapper = mapper;
        }

        public async Task<List<string>> AllRoles()
        => await roleManager.Roles
                .Select(x => x.Name)
                .Where(x => x != "Administrator")
                .OrderBy(x => x)
                .ToListAsync();

        public async Task<bool> CheckUserPassword(User user, string password)
            => await userManager
                    .CheckPasswordAsync(user, password);

        public async Task<User> FindUserByUserName(string userName)
            => await userManager
                    .FindByNameAsync(userName);

        public async Task<IdentityResult> CreateUser(string userName, string email, string password)
        {
            var user = new User
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true,
            };

            var result = await userManager
                .CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(x => x.Description)));
            }

            var roleResult = await userManager
                            .AddToRoleAsync(user, "User");

            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(CouldNotAddRole);
            }

            return result;
        }

        public async Task<ProfileServiceModel?> FindUserById(string id)
            => await db.Users
                .Where(x => x.Id == id)
                .ProjectTo<ProfileServiceModel>(mapper!.ConfigurationProvider)
                .FirstOrDefaultAsync();

        public async Task<string> GetEncryptedToken(User user)
        {
            var roles = await userManager
                            .GetRolesAsync(user);

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

            var key = Encoding.ASCII
                .GetBytes(appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow
                .AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                        SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler
                .CreateToken(tokenDescriptor);

            var encryptedToken = tokenHandler
                .WriteToken(token);

            return encryptedToken;
        }
    }
}
