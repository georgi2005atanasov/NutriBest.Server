namespace NutriBest.Server.Features.Profile.Models
{
    public class AllProfilesServiceModel
    {
        public int TotalUsers { get; set; }

        public List<ProfileListingServiceModel> Profiles { get; set; } = new List<ProfileListingServiceModel>();
    }
}
