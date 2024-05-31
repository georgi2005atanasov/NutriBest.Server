namespace NutriBest.Server.Features.Orders
{
    using NutriBest.Server.Features.Carts.Models;

    public interface IOrderService
    {
        Task<int> PrepareCart(decimal totalPrice,
            decimal originalPrice,
            decimal totalSaved,
            string? code,
            List<CartProductServiceModel> cartProducts);

        Task<CartServiceModel?> GetFinishedOrder(int orderId);
    }
}
