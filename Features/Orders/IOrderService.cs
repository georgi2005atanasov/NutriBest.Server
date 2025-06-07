namespace NutriBest.Server.Features.Orders
{
    using NutriBest.Server.Features.Carts.Models;
    using NutriBest.Server.Features.Orders.Models;

    public interface IOrderService
    {
        Task<int> PrepareCart(decimal totalProducts,
            decimal originalPrice,
            decimal totalSaved,
            string? code,
            List<CartProductServiceModel> cartProducts);

        Task<AllOrdersServiceModel> All(int page, string? search, string? filters, DateTime? startDate, DateTime? endDate);

        Task<List<OrderListingServiceModel>> AllForExport(string? search, string? filters, DateTime? startDate, DateTime? endDate);

        Task<OrdersSummaryServiceModel> Summary();

        Task<AllOrdersServiceModel> Mine(int page, string? search);

        Task<OrderRelatedProductsServiceModel> GetOrderRelatedProducts(decimal price);

        Task<OrderServiceModel?> GetFinishedOrder(int orderId, string? token);

        Task<OrderServiceModel?> GetFinishedByAdmin(int orderId);

        Task<bool> ChangeStatuses(int orderId,
            bool isFinished,
            bool isPaid,
            bool isShipped,
            bool isConfirmed);

        Task DeleteById(int orderId); 

        Task<bool> ConfirmOrder(int orderId);

        Task SetShippingPrice(int cartId, string country);
    }
}
