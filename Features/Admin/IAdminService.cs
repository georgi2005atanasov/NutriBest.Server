namespace NutriBest.Server.Features.Admin
{
    using NutriBest.Server.Features.Admin.Models;

    public interface IAdminService
    {
        Task<IEnumerable<UserServiceModel>> GetAllUsers();
    }
}
