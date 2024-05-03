﻿
namespace NutriBest.Server.Infrastructure.Services
{
    using NutriBest.Server.Infrastructure.Extensions;
    using System.Security.Claims;

    public class CurrentUserService : ICurrentUserService
    {
        private readonly ClaimsPrincipal user;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor) 
            => this.user = httpContextAccessor.HttpContext?.User;

        public string? GetUserId() 
            => this.user.GetId();

        public string? GetUserName()
            => user?.Identity?.Name;
    }
}