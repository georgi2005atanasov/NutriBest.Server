namespace NutriBest.Server.Features.Profile.Models
{
    public class ProfileServiceModel
    {
        public string? Email { get; set; }

        public string? UserName { get; set; }

        public string? Name { get; set; }

        public string? Gender { get; set; }

        public int? Age { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
