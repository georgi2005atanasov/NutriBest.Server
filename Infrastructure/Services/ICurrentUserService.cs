namespace NutriBest.Server.Infrastructure.Services
{
    public interface ICurrentUserService
    {
        string? GetUserId();
        string? GetUserName();
    }
}
