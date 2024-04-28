namespace NutriBest.Server.Features.Carts.Models
{
    public class CartServiceModel
    {
        public List<CartProductServiceModel>? CartProducts { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
