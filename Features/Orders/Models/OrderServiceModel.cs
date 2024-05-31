namespace NutriBest.Server.Features.Orders.Models
{
    using NutriBest.Server.Features.Carts.Models;

    public class OrderServiceModel
    {
        public CartServiceModel? Cart { get; set; }
    }
}
