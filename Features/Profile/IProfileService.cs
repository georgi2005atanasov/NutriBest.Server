namespace NutriBest.Server.Features.Admin
{
    public interface IProfileService
    {
        Task<string> UpdateProfile(string? name,
            string? userName,
            string? email,
            int? age,
            string? gender);
    }
}
