namespace NutriBest.Server.Infrastructure.Services
{
    using System.Security.Claims;
    using NutriBest.Server.Infrastructure.Extensions;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;

    public class CurrentUserService : ICurrentUserService, IScopedService
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
