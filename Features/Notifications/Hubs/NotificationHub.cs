namespace NutriBest.Server.Features.Notifications.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using System.Collections.Concurrent;

    public class NotificationHub : Hub
    {
        public const string AdminGroup = "AdminGroup";
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, bool>> PageConnections = new ConcurrentDictionary<string, ConcurrentDictionary<string, bool>>()
        {
            ["/"] = new ConcurrentDictionary<string, bool>(),
            ["/login"] = new ConcurrentDictionary<string, bool>(),
            ["/forgot-password"] = new ConcurrentDictionary<string, bool>(),
            ["/Identity/ResetPassword"] = new ConcurrentDictionary<string, bool>(),
            ["/register"] = new ConcurrentDictionary<string, bool>(),
            ["/logout"] = new ConcurrentDictionary<string, bool>(),
            ["/home"] = new ConcurrentDictionary<string, bool>(),
            ["/products"] = new ConcurrentDictionary<string, bool>(),
            ["/products/add"] = new ConcurrentDictionary<string, bool>(),
            ["/products/all"] = new ConcurrentDictionary<string, bool>(),
            ["/profile"] = new ConcurrentDictionary<string, bool>(),
            ["/profile/:id"] = new ConcurrentDictionary<string, bool>(),
            ["/profile/address"] = new ConcurrentDictionary<string, bool>(),
            ["/profiles"] = new ConcurrentDictionary<string, bool>(),
            ["/promotions"] = new ConcurrentDictionary<string, bool>(),
            ["/promotions/add"] = new ConcurrentDictionary<string, bool>(),
            ["/categories"] = new ConcurrentDictionary<string, bool>(),
            ["/categories/add"] = new ConcurrentDictionary<string, bool>(),
            ["/brands"] = new ConcurrentDictionary<string, bool>(),
            ["/brands/add"] = new ConcurrentDictionary<string, bool>(),
            ["/flavours"] = new ConcurrentDictionary<string, bool>(),
            ["/flavours/add"] = new ConcurrentDictionary<string, bool>(),
            ["/packages"] = new ConcurrentDictionary<string, bool>(),
            ["/packages/add"] = new ConcurrentDictionary<string, bool>(),
            ["/more"] = new ConcurrentDictionary<string, bool>(),
            ["/cart"] = new ConcurrentDictionary<string, bool>(),
            ["/promo-codes"] = new ConcurrentDictionary<string, bool>(),
            ["/promo-codes/add"] = new ConcurrentDictionary<string, bool>(),
            ["/order"] = new ConcurrentDictionary<string, bool>(),
            ["/order/finished"] = new ConcurrentDictionary<string, bool>(),
            ["/order/confirm"] = new ConcurrentDictionary<string, bool>(),
            ["/orders"] = new ConcurrentDictionary<string, bool>(),
            ["/my-orders"] = new ConcurrentDictionary<string, bool>(),
            ["/shipping-discounts"] = new ConcurrentDictionary<string, bool>(),
            ["/shipping-discounts/add"] = new ConcurrentDictionary<string, bool>(),
            ["/shipping-discounts/all"] = new ConcurrentDictionary<string, bool>(),
            ["/newsletter/list"] = new ConcurrentDictionary<string, bool>(),
            ["/live/dashboard"] = new ConcurrentDictionary<string, bool>(),
            ["/notifications"] = new ConcurrentDictionary<string, bool>(),
            ["/addToNewsletter"] = new ConcurrentDictionary<string, bool>()
        };
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        // I HAVE PUT QUESTION MARK BELOW ON EXCEPTION WITHOUT TESTING IT!!!
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connections = PageConnections.Values;

            foreach (var connection in connections)
            {
                connection.TryRemove(Context.ConnectionId, out _);
            }

            await GetUsersCount();
            await base.OnDisconnectedAsync(exception);
        }

        public Task NotifyAdmin(string notificationType, string message)
        {
            return Clients.Group("AdminGroup").SendAsync("NotifyAdmin", notificationType, message);
        }

        public Task NotifyLowStock(string priority, string message, string productId)
        {
            return Clients.Group("AdminGroup").SendAsync("NotifyLowStock", priority, message, productId);
        }

        public async Task JoinPage(string pageName)
        {
            var currentConnections = PageConnections.GetOrAdd(pageName, _ => new ConcurrentDictionary<string, bool>());

            foreach (var connections in PageConnections.Values)
            {
                connections.TryRemove(Context.ConnectionId, out _);
            }

            currentConnections.TryAdd(Context.ConnectionId, true);

            await GetUsersCount();
            await Groups.AddToGroupAsync(Context.ConnectionId, AdminGroup);
        }

        public async Task GetUsersCount()
            => await Clients.Group(AdminGroup).SendAsync("GetUsersCount", PageConnections
                .Where(x => x.Value.Count > 0)
                .Select(x => new
                {
                    Route = x.Key,
                    Count = x.Value.Count
                })
                .ToList());
    }
}
