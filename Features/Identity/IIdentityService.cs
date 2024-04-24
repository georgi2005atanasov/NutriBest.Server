﻿namespace NutriBest.Server.Features.Identity
{
    using Microsoft.AspNetCore.Identity;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Identity.Models;

    public interface IIdentityService
    {
        Task<string> GetEncryptedToken(User user);

        Task<IdentityResult> CreateUser(string userName, string email, string password);

        Task<User> FindUserByName(string userName);

        Task<UserServiceModel> FindUserById(string id);

        Task<bool> CheckUserPassword(User user, string password);
    }
}
