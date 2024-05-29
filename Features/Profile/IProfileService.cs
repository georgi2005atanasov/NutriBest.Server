using NutriBest.Server.Features.Profile.Models;

namespace NutriBest.Server.Features.Admin
{
    public interface IProfileService
    {
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
