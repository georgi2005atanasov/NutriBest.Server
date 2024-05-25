namespace NutriBest.Server.Features.Carts.Models
{
    public class CartServiceModel
    {
        public List<CartProductServiceModel> CartProducts { get; set; } = new List<CartProductServiceModel>();

        public string? Code { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal TotalSaved { get; set; }
    }
}
