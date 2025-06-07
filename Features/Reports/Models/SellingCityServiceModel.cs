namespace NutriBest.Server.Features.Reports.Models
{
    public class SellingCityServiceModel
    {
        public string Country { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public int SoldCount { get; set; }
    }
}
