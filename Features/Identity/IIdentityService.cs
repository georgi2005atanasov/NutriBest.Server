using Microsoft.AspNetCore.Identity;
using NutriBest.Server.Data.Models;

namespace NutriBest.Server.Features.Identity
{
    public interface IIdentityService
    {
        Task<string> GetEncryptedToken(User user);

        Task<IdentityResult> CreateUser(string userName, string email, string password);

        Task<User> FindUserByName(string userName);

        Task<bool> CheckUserPassword(User user, string password);
    }
}
