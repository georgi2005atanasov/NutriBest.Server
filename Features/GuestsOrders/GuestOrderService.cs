namespace NutriBest.Server.Features.Orders
{
    using AutoMapper;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Notifications;
    using NutriBest.Server.Infrastructure.Services;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;

    public class GuestOrderService : OrderService, IGuestOrderService, ITransientService
    {
        public GuestOrderService(NutriBestDbContext db,
            ICurrentUserService currentUserService,
            IConfiguration config,
            INotificationService notificationService,
            IMapper mapper)
            :base(db, currentUserService, config, notificationService, mapper)
        {
        }

        public async Task<int> CreateGuestOrder(int orderId, 
            string name,
            string email,
            string? phoneNumber)
        {
            var guestOrder = new GuestOrder
            {
                OrderId = orderId,
                Name = name,
                Email = email,
                PhoneNumber = phoneNumber
            };

            this.db.GuestsOrders.Add(guestOrder);

            await this.db.SaveChangesAsync();

            return guestOrder.Id;
        }
    }
}
