namespace NutriBest.Server.Features.Notifications.Models
{
    public class AllNotificationsServiceModel
    {
        public List<NotificationServiceModel> Notifications { get; set; } = new List<NotificationServiceModel>();

        public int TotalNotifications { get; set; }
    }
}
