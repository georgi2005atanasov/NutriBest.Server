namespace NutriBest.Server.Features.Cities.Models
{
    public class AllCitiesServiceModel
    {
        public string Country { get; set; } = null!;

        public List<CityServiceModel>? Cities { get; set; }

        public decimal ShippingPrice { get; set; }
        
        public decimal? MinimumPriceForDiscount { get; set; }

        public decimal ShippingPriceWithDiscount { get; set; }
    }
}
