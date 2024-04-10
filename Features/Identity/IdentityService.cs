namespace NutriBest.Server.Features.Identity
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using NutriBest.Server.Data.Models;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> userManager;
        private readonly ApplicationSettings appSettings;

        public IdentityService(UserManager<User> userManager,
            IOptions<ApplicationSettings> appSettings)
        {
            this.userManager = userManager;
            this.appSettings = appSettings.Value;
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

            return result;
        }

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
