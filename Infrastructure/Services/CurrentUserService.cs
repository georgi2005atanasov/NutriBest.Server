using NutriBest.Server.Infrastructure.Extensions;
using System.Security.Claims;

namespace NutriBest.Server.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly ClaimsPrincipal user;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
            => this.user = httpContextAccessor.HttpContext?.User;

        public string? GetUserId()
            => user
            .GetId();

        public string? GetUserName()
            => user
            .Identity?
            .Name;
    }
}
