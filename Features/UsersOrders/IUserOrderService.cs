namespace NutriBest.Server.Features.UsersOrders
{
    using NutriBest.Server.Features.Orders;

    public interface IUserOrderService : IOrderService
    {
        Task<int> CreateUserOrder(string userId,
            int orderId,
            string name,
            string email,
            string? phoneNumber);
    }
}
