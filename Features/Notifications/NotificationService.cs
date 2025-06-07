using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Notifications
{
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Notifications.Hubs;
    using NutriBest.Server.Features.Notifications.Models;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;
    using static ServicesConstants.PaginationConstants;
    using static ServicesConstants.Product;
    using static ErrorMessages.NotificationService;

    public class NotificationService : INotificationService, ITransientService
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
                                        .OrderByDescending(x => x.SentAt)
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

            if (quantity < StockMediumPriority && quantity > 0)
            {
                notification.Title = StockIsRunningLow;
                notification.Message = string.Format(CriticallyLowStockLevels, productName, quantity); ;
                notification.Priority = Data.Enums.Priority.Medium;
                notification.ProductId = productId;
                db.Notifications.Add(notification);
                await db.SaveChangesAsync();
                await hubContext.Clients.All.SendAsync("NotifyLowStock", notification.Priority.ToString(), notification.Message);
            }
            else if (quantity < StockLowPriority && quantity > 0)
            {
                notification.Title = LowInStock;
                notification.Message = string.Format(BeAwareOfTheProductQuantity, productName, quantity); ;
                notification.Priority = Data.Enums.Priority.Low;
                notification.ProductId = productId;
                db.Notifications.Add(notification);
                await db.SaveChangesAsync();
                await hubContext.Clients.All.SendAsync("NotifyLowStock", notification.Priority.ToString(), notification.Message);
            }
            else if (quantity == 0)
            {
                notification.Title = OutOfStock;
                notification.Message = string.Format(OutOfStockMesage, productName, quantity);
                notification.Priority = Data.Enums.Priority.High;
                notification.ProductId = productId;
                db.Notifications.Add(notification);
                await db.SaveChangesAsync();
                await hubContext.Clients.All.SendAsync("NotifyLowStock", notification.Priority.ToString(), notification.Message, productId);
            }
            else if (quantity < 0)
            {
                notification.Priority = Data.Enums.Priority.High;
                await hubContext.Clients.All.SendAsync("NotifyLowStock", notification.Priority.ToString(), string.Format(CannotFulfillOrder, $"#000000{orderId}", productName), productId);
                throw new InvalidOperationException(string.Format(CannotFulfillOrder, $"#000000{orderId}", productName));
            }
        }

        public async Task SendNotificationToAdmin(string notificationType, string message)
        {
            await hubContext.Clients.All.SendAsync("NotifyAdmin", notificationType, message);
        }
    }
}
