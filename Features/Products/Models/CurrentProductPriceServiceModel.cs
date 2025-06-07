namespace NutriBest.Server.Features.Products.Models
{
    public class CurrentProductPriceServiceModel
    {
        public int ProductId { get; set; }

        public string Flavour { get; set; } = null!;

        public int Package { get; set; }
    }
}
