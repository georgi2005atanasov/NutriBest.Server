namespace NutriBest.Server.Features.Profile.Models
{
    public class ProfileListingServiceModel
    {
        public string? Email { get; set; }

        public DateTime MadeOn { get; set; }

        public string? PhoneNumber { get; set; }

        public string? City { get; set; }

        public int TotalOrders { get; set; }

        public string? Name { get; set; }
    }
}
