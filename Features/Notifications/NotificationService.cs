namespace NutriBest.Server.Features.Notifications
{
    using Microsoft.AspNetCore.SignalR;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Notifications.Hubs;

    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> hubContext;
        private readonly NutriBestDbContext db;

        public NotificationService(IHubContext<NotificationHub> hubContext, NutriBestDbContext db)
        {
            this.hubContext = hubContext;
            this.db = db;
        }

        public Task<bool> DeleteNotification(string message)
        {
            throw new NotImplementedException();
        }

        public async Task SendLowInStockNotification(string productName, int productId, int quantity)
        {
            var notification = new Notification()
            {
                SentAt = DateTime.UtcNow
            };
            if (quantity < 10)
            {
                notification.Title = "Low in Stock";
                notification.Message = $"'{productName}' stock levels are critically low!";
                notification.Priority = Data.Enums.Priority.Medium;
                db.Notifications.Add(notification);
                await db.SaveChangesAsync();
                await hubContext.Clients.All.SendAsync("NotifyLowStock", notification.Priority.ToString(), notification.Message);
            }
            else if (quantity < 20)
            {
                notification.Title = "Stock is running low!";
                notification.Message = $"Be Aware That Product With Name '{productName}' has Quantity of {quantity}.";
                notification.Priority = Data.Enums.Priority.Low;
                db.Notifications.Add(notification);
                await db.SaveChangesAsync();
                await hubContext.Clients.All.SendAsync("NotifyLowStock", notification.Priority.ToString(), notification.Message);
            }
            else if (quantity <= 0)
            {
                notification.Title = "Out of Stock!";
                notification.Message = $"'{productName}' is Out of Stock! ({quantity})";
                notification.Priority = Data.Enums.Priority.High;
                db.Notifications.Add(notification);
                await db.SaveChangesAsync();
                await hubContext.Clients.All.SendAsync("NotifyLowStock", notification.Priority.ToString(), notification.Message, productId);
            }
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
