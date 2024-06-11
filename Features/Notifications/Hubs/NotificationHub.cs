namespace NutriBest.Server.Features.Notifications.Hubs
{
    using Microsoft.AspNetCore.SignalR;

    public class NotificationHub : Hub
    {
        public const string AdminGroup = "AdminGroup";

        public async Task AddToAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, AdminGroup);
        }

        public async Task RemoveFromAdminGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, AdminGroup);
        }

        public Task NotifyAdmin(string notificationType, string message)
        {
            return Clients.Group("AdminGroup").SendAsync("ReceiveNotification", notificationType, message);
        }
    }
}
