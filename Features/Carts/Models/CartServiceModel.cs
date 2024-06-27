namespace NutriBest.Server.Features.Carts.Models
{
    public class CartServiceModel
    {
        public string? Code { get; set; }

        public decimal TotalProducts { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal TotalSaved { get; set; }

        public decimal? ShippingPrice { get; set; }

        public List<CartProductServiceModel> CartProducts { get; set; } = new List<CartProductServiceModel>();
    }
}
