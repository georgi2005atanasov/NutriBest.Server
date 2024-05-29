namespace NutriBest.Server.Features.Profile.Models
{
    public class ProfileAddressServiceModel
    {
        public string Street { get; set; } = null!;

        public string? StreetNumber { get; set; }

        public string Country { get; set; } = null!;

        public string City { get; set; } = null!;

        public int? PostalCode { get; set; }
    }
}
