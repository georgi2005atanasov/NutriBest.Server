namespace NutriBest.Server.Features.Notifications
{
    using Microsoft.AspNetCore.SignalR;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Notifications.Hubs;

    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> hubContext;
        private readonly NutriBestDbContext db;

        public NotificationService(IHubContext<NotificationHub> hubContext, NutriBestDbContext db)
        {
            this.hubContext = hubContext;
            this.db= db;
        }

        public async Task SendNotificationToAdmin(string notificationType, string message)
        {
            // Store notification in the database
            //var notification = new Notification
            //{
            //    Type = notificationType,
            //    Message = message,
            //    Timestamp = DateTime.UtcNow
            //};
            //_dbContext.Notifications.Add(notification);
            //await _dbContext.SaveChangesAsync();

            // Send notification to connected clients

            await hubContext.Clients.All.SendAsync("NotifyAdmin", notificationType, message);
        }
    }
}
