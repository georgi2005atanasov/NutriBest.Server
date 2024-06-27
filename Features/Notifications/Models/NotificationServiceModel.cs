namespace NutriBest.Server.Features.Notifications.Models
{
    public class NotificationServiceModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Message { get; set; } = null!;

        public DateTime SentAt { get; set; }

        public string Priority { get; set; } = null!;

        public int? ProductId { get; set; }
    }
}
