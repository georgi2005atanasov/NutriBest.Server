namespace NutriBest.Server.Features.Identity
{
    using Microsoft.AspNetCore.Identity;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Profile.Models;

    public interface IIdentityService
    {
        Task<string> GetEncryptedToken(User user);

        Task<IdentityResult> CreateUser(string userName, string email, string password);

        Task<User> FindUserByUserName(string userName);

        Task<ProfileServiceModel?> FindUserById(string id);

        Task<bool> CheckUserPassword(User user, string password);

        Task<List<string>> AllRoles(); 
    }
}
