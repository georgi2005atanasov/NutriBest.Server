namespace NutriBest.Server.Features.Notifications
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Notifications.Hubs;
    using NutriBest.Server.Features.Notifications.Models;
    using static ServicesConstants.PaginationConstants;

    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> hubContext;
        private readonly NutriBestDbContext db;
        private readonly IMapper mapper;

        public NotificationService(IHubContext<NotificationHub> hubContext,
            NutriBestDbContext db,
            IMapper mapper)
        {
            this.hubContext = hubContext;
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<AllNotificationsServiceModel> All(int page)
        {
            var notifications = db.Notifications
                .Where(x => !x.IsDeleted)
                .ProjectTo<NotificationServiceModel>(mapper.ConfigurationProvider)
                .OrderByDescending(x => x.SentAt);

            var allNotificationsServiceModel = new AllNotificationsServiceModel
            {
                TotalNotifications = await notifications.CountAsync(),
                Notifications = await notifications
                                        .Skip((page - 1) * NotificationsPerPage)
                                        .Take(NotificationsPerPage)
                                        .ToListAsync()
            };

            return allNotificationsServiceModel;
        }

        public async Task<bool> DeleteNotification(string message)
        {
            var notification = await db.Notifications
                .FirstOrDefaultAsync(x => x.Message == message);

            if (notification == null)
                return false;

            notification.IsDeleted = true;

            await db.SaveChangesAsync();

            return true;
        }

        public async Task SendLowInStockNotification(string productName, int productId, int quantity, string orderId)
        {
            var notification = new Notification()
            {
                SentAt = DateTime.UtcNow
            };

            if (await db.Notifications.AnyAsync(x => x.ProductId == productId))
            {
                var notificationToRemove = await db.Notifications
                    .FirstAsync(x => x.ProductId == productId);

                notificationToRemove!.IsDeleted = true;
            }

            if (quantity < 10 && quantity > 0)
            {
                notification.Title = "Low in Stock";
                notification.Message = $"'{productName}' stock levels are critically low! ({quantity} left)";
                notification.Priority = Data.Enums.Priority.Medium;
                notification.ProductId = productId;
                db.Notifications.Add(notification);
                await db.SaveChangesAsync();
                await hubContext.Clients.All.SendAsync("NotifyLowStock", notification.Priority.ToString(), notification.Message);
            }
            else if (quantity < 20 && quantity > 0)
            {
                notification.Title = "Stock is running low!";
                notification.Message = $"Be Aware That Product With Name '{productName}' has Quantity of {quantity}.";
                notification.Priority = Data.Enums.Priority.Low;
                notification.ProductId = productId;
                db.Notifications.Add(notification);
                await db.SaveChangesAsync();
                await hubContext.Clients.All.SendAsync("NotifyLowStock", notification.Priority.ToString(), notification.Message);
            }
            else if (quantity == 0)
            {
                notification.Title = "Out of Stock!";
                notification.Message = $"'{productName}' is Out of Stock! ({quantity})";
                notification.Priority = Data.Enums.Priority.High;
                notification.ProductId = productId;
                db.Notifications.Add(notification);
                await db.SaveChangesAsync();
                await hubContext.Clients.All.SendAsync("NotifyLowStock", notification.Priority.ToString(), notification.Message, productId);
            }
            else if (quantity < 0)
            {
                notification.Priority = Data.Enums.Priority.High;
                await hubContext.Clients.All.SendAsync("NotifyLowStock", notification.Priority.ToString(), $"Cannot Fulfill the Order. Product '{productName}' is Out of Stock!", productId);
                throw new InvalidOperationException($"Cannot Fulfill Order {orderId}. Product '{productName}' is Out of Stock!");
            }
        }

        public async Task SendNotificationToAdmin(string notificationType, string message)
        {
            await hubContext.Clients.All.SendAsync("NotifyAdmin", notificationType, message);
        }
    }
}
