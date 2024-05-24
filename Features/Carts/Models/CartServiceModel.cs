namespace NutriBest.Server.Features.Carts.Models
{
    public class CartServiceModel
    {
        public List<CartProductServiceModel> CartProducts { get; set; } = new List<CartProductServiceModel>();

        public decimal TotalPrice { get; set; }

        public decimal TotalSaved { get; set; }
    }
}
