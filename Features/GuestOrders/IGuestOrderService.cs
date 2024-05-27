namespace NutriBest.Server.Features.Orders
{
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Carts.Models;

    public interface IGuestOrderService
    {
        Task<int> CreateGuestOrder(int orderId, 
            string name, 
            string email,
            string? phoneNumber);

        Task<int> PrepareCart(decimal totalPrice,
            decimal originalPrice,
            decimal totalSaved,
            string? code,
            List<CartProductServiceModel> cartProducts);
    }
}
