namespace NutriBest.Server.Features.Profile.Models
{
    public class ProfileServiceModel
    {
        public string? Name { get; set; }

        public string? Gender { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
