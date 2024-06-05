namespace NutriBest.Server.Features.Profile.Models
{
    public class AllProfilesServiceModel
    {
        public List<ProfileListingServiceModel> Profiles { get; set; } = new List<ProfileListingServiceModel>();

        public int TotalUsers { get; set; }
    }
}
