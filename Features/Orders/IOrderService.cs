namespace NutriBest.Server.Features.Orders
{
    public interface IOrderService
    {
        Task<int> CreateOrder();
    }
}
