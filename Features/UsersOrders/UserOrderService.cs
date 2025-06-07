namespace NutriBest.Server.Features.UsersOrders
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using AutoMapper;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Notifications;
    using NutriBest.Server.Features.Orders;
    using NutriBest.Server.Infrastructure.Services;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;

    public class UserOrderService : OrderService, ITransientService, IUserOrderService
    {
        public UserOrderService(NutriBestDbContext db,
            ICurrentUserService currentUserService,
            IConfiguration config,
            INotificationService notificationService,
            IMapper mapper)
            : base(db, currentUserService, config, notificationService, mapper)
        {
        }

        public async Task<int> CreateUserOrder(string userId, int orderId, string name, string email, string? phoneNumber)
        {
            var profile = await db.Profiles
                .FirstAsync(x => x.UserId == userId);

            var user = await db.Users
                .FirstAsync(x => x.Id == userId);

            if (string.IsNullOrEmpty(profile.Name) || !string.IsNullOrEmpty(name))
                profile.Name = name;

            if (string.IsNullOrEmpty(user.Email) || !string.IsNullOrEmpty(email))
                user.Email = email;

            if (string.IsNullOrEmpty(user.PhoneNumber) || !string.IsNullOrEmpty(phoneNumber))
                user.PhoneNumber = phoneNumber;

            var userOrder = new UserOrder
            {
                ProfileId = userId,
                OrderId = orderId,
            };

            this.db.UsersOrders.Add(userOrder);

            await this.db.SaveChangesAsync();

            return userOrder.Id;
        }
    }
}
