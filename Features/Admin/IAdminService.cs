namespace NutriBest.Server.Features.Admin
{
    using NutriBest.Server.Features.Admin.Models;

    public interface IAdminService
    {
        Task<IEnumerable<UserServiceModel>> GetAllUsers();

        Task<string> RestoreProfile(string id);

        Task<bool> DeleteProfile(string id);
    }
}
