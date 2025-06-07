namespace NutriBest.Server.Features.Notifications
{
    using NutriBest.Server.Features.Notifications.Models;

    public interface INotificationService
    {
        Task<AllNotificationsServiceModel> All(int page);

        Task SendNotificationToAdmin(string notificationType, string message);

        Task SendLowInStockNotification(string productName,
            int productId, int quantity, string orderId);

        Task<bool> DeleteNotification(string message);
    }
}
