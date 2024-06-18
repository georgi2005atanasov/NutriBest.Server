﻿namespace NutriBest.Server.Features.UserOrders
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Notifications;
    using NutriBest.Server.Features.Orders;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;
    using NutriBest.Server.Infrastructure.Services;
    using System.Threading.Tasks;

    public class UserOrderService : OrderService, IUserOrderService, ITransientService
    {
        public UserOrderService(NutriBestDbContext db,
            ICurrentUserService currentUserService,
            IConfiguration config,
            INotificationService notificationService,
            IMapper mapper)
            :base(db, currentUserService, config, notificationService, mapper)
        {
        }

        public async Task<int> CreateUserOrder(string userId, int orderId, string name, string email, string? phoneNumber)
        {
            var profile = await db.Profiles
                .FirstAsync(x => x.UserId == userId);

            var user = await db.Users
                .FirstAsync(x => x.Id == userId);

            if (string.IsNullOrEmpty(profile.Name))
                profile.Name = name;

            if (string.IsNullOrEmpty(user.Email))
                user.Email = email;

            if (string.IsNullOrEmpty(user.PhoneNumber))
                user.PhoneNumber = phoneNumber;

            var userOrder = new UserOrder
            {
                ProfileId = userId,
                OrderId = orderId
            };

            this.db.UsersOrders.Add(userOrder);

            await this.db.SaveChangesAsync();

            return userOrder.Id;
        }
    }
}
