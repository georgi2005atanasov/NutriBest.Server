namespace NutriBest.Server.Features.Admin
{
    using NutriBest.Server.Features.Profile.Models;

    public interface IProfileService
    {
        Task<AllProfilesServiceModel> All(int page, string? search, string? groupType);

        Task<List<ProfileListingServiceModel>> AllForExport(string? search, string? groupType);

        Task<ProfileDetailsServiceModel> GetDetailsById(string id);

        Task<string> UpdateProfile(string? name,
            string? userName,
            string? email,
            int? age,
            string? gender);

        Task<ProfileAddressServiceModel> GetAddress();

        Task<int> SetAddress(string street,
            string? streetNumber,
            string city,
            string country,
            int? postalCode);
    }
}
