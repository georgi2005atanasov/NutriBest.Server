namespace NutriBest.Server.Features.Notifications.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using System.Collections.Concurrent;

    public class NotificationHub : Hub
    {
        public const string AdminGroup = "AdminGroup";
        private static readonly ConcurrentDictionary<string, HashSet<string>> PageConnections = new ConcurrentDictionary<string, HashSet<string>>()
        {
            ["/"] = new HashSet<string>(),
            ["/login"] = new HashSet<string>(),
            ["/forgot-password"] = new HashSet<string>(),
            ["/Identity/ResetPassword"] = new HashSet<string>(),
            ["/register"] = new HashSet<string>(),
            ["/logout"] = new HashSet<string>(),
            ["/home"] = new HashSet<string>(),
            ["/products"] = new HashSet<string>(),
            ["/products/add"] = new HashSet<string>(),
            ["/products/all"] = new HashSet<string>(),
            ["/profile"] = new HashSet<string>(),
            ["/profile/:id"] = new HashSet<string>(),
            ["/profile/address"] = new HashSet<string>(),
            ["/profiles"] = new HashSet<string>(),
            ["/promotions"] = new HashSet<string>(),
            ["/promotions/add"] = new HashSet<string>(),
            ["/categories"] = new HashSet<string>(),
            ["/categories/add"] = new HashSet<string>(),
            ["/brands"] = new HashSet<string>(),
            ["/brands/add"] = new HashSet<string>(),
            ["/flavours"] = new HashSet<string>(),
            ["/flavours/add"] = new HashSet<string>(),
            ["/packages"] = new HashSet<string>(),
            ["/packages/add"] = new HashSet<string>(),
            ["/more"] = new HashSet<string>(),
            ["/cart"] = new HashSet<string>(),
            ["/promo-codes"] = new HashSet<string>(),
            ["/promo-codes/add"] = new HashSet<string>(),
            ["/order"] = new HashSet<string>(),
            ["/order/finished"] = new HashSet<string>(),
            ["/order/confirm"] = new HashSet<string>(),
            ["/orders"] = new HashSet<string>(),
            ["/my-orders"] = new HashSet<string>(),
            ["/shipping-discounts"] = new HashSet<string>(),
            ["/shipping-discounts/add"] = new HashSet<string>(),
            ["/shipping-discounts/all"] = new HashSet<string>()
        };
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            foreach (var page in PageConnections)
            {
                if (page.Value.Remove(Context.ConnectionId))
                {
                    await GetUsersCount();
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinPage(string pageName)
        {
            if (PageConnections.ContainsKey(pageName))
            {
                PageConnections.AddOrUpdate(pageName,
                    addValueFactory: _ => new HashSet<string> { Context.ConnectionId },
                    updateValueFactory: (key, oldValue) => { oldValue.Add(Context.ConnectionId); return oldValue; });

                await GetUsersCount();
                await Groups.AddToGroupAsync(Context.ConnectionId, AdminGroup);
            }
        }

        public async Task LeavePage(string pageName)
        {
            if (PageConnections.TryGetValue(pageName, out var connections))
            {
                connections.Remove(Context.ConnectionId);
                await GetUsersCount();
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, AdminGroup);
        }

        private async Task GetUsersCount()
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
