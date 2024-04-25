
namespace NutriBest.Server.Infrastructure.Extensions
{
    using System.Security.Claims;

    public static class IdentityExtensions
    {
        public static string? GetId(this ClaimsPrincipal user)
        {
            foreach (var claim in user.Claims)
            {
                Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
            }

            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?.Value;
        }
    }
}
