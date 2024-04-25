namespace NutriBest.Server.Features.Admin
{
    public interface IProfileService
    {
        Task<bool> UpdateUserProfile();
    }
}
