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

        Task<AllOrdersServiceModel> Mine();

        Task<OrderServiceModel?> GetFinishedOrder(int orderId, string? token);

        Task<OrderServiceModel?> GetFinishedByAdmin(int orderId);

        Task<bool> ChangeStatuses(int orderId,
            bool isFinished,
            bool isPaid,
            bool isShipped);

        Task DeleteById(int orderId); 

        Task<bool> ConfirmOrder(int orderId);
    }
}
