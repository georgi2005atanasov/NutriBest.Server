namespace NutriBest.Server.Features.Profile.Models
{
    public class ProfileDetailsServiceModel : ProfileListingServiceModel
    {
        public string? Street { get; set; }

        public string? StreetNumber { get; set; }

        public string? Country { get; set; }

        public string? Gender { get; set; }

        public int? Age { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
