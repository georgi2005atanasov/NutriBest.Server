namespace NutriBest.Server.Features.Notifications.Models
{
    public class AllNotificationsServiceModel
    {
        public int TotalNotifications { get; set; }

        public List<NotificationServiceModel> Notifications { get; set; } = new List<NotificationServiceModel>();
    }
}
