using NutriBest.Server.Features.Carts.Models;

namespace NutriBest.Server.Features.Orders
{
    public interface IOrderService
    {
        Task<int> PrepareCart(decimal totalPrice,
            decimal originalPrice,
            decimal totalSaved,
            string? code,
            List<CartProductServiceModel> cartProducts);
    }
}
