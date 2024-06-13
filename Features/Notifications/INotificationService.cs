﻿namespace NutriBest.Server.Features.Notifications
{
    using NutriBest.Server.Features.Notifications.Models;

    public interface INotificationService
    {
        Task SendNotificationToAdmin(string notificationType, string message);
        
        Task SendLowInStockNotification(string productName,
            int productId, int quantity);

        Task<bool> DeleteNotification(string message);
    }
}
