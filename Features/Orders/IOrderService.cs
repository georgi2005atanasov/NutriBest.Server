namespace NutriBest.Server.Features.Orders
{
    using NutriBest.Server.Features.Carts.Models;
    using NutriBest.Server.Features.Orders.Models;

    public interface IOrderService
    {
        Task<int> PrepareCart(decimal totalPrice,
            decimal originalPrice,
            decimal totalSaved,
            string? code,
            List<CartProductServiceModel> cartProducts);

        Task<AllOrdersServiceModel> All(int page, string? search);

        Task<OrderServiceModel?> GetFinishedOrder(int orderId, string? token);

        Task<bool> ConfirmOrder(int orderId);
    }
}
