namespace NutriBest.Server.Features.Notifications
{
    public interface INotificationService
    {
        Task SendNotificationToAdmin(string notificationType, string message);
    }
}
