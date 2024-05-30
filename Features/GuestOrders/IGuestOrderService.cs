namespace NutriBest.Server.Features.Orders
{
    public interface IGuestOrderService : IOrderService
    {
        Task<int> CreateGuestOrder(int orderId, 
            string name, 
            string email,
            string? phoneNumber);
    }
}
