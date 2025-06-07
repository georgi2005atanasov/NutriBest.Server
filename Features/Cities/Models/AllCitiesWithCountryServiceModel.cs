namespace NutriBest.Server.Features.Cities.Models
{
    public class AllCitiesWithCountryServiceModel
    {
        public string Country { get; set; } = null!;

        public decimal ShippingPrice { get; set; }
        
        public decimal? MinimumPriceForDiscount { get; set; }

        public decimal ShippingPriceWithDiscount { get; set; }

        public List<CityServiceModel>? Cities { get; set; }
    }
}
