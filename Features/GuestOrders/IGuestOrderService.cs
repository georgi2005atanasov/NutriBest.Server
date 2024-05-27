namespace NutriBest.Server.Features.Orders
{
    public interface IGuestOrderService
    {
        Task<int> CreateOrder();
    }
}
