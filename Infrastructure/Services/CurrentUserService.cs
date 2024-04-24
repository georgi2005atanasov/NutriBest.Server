
namespace NutriBest.Server.Infrastructure.Services
{
    using NutriBest.Server.Infrastructure.Extensions;
    using System.Security.Claims;

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string? GetUserId()
        {
            var user = httpContextAccessor.HttpContext?.User;
            return user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public string? GetUserName()
        {
            var user = httpContextAccessor.HttpContext?.User;
            return user?.Identity?.Name;
        }
    }
}
