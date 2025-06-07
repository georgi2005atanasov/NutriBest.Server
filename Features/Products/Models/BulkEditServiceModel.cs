namespace NutriBest.Server.Features.Products.Models
{
    public class BulkEditServiceModel
    {
        public string Category { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public bool HasPromotion { get; set; }

        public string PriceChange { get; set; } = string.Empty;

        public int QuantityChange { get; set; }
    }
}
