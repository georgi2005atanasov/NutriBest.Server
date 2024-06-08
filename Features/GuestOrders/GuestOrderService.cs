﻿namespace NutriBest.Server.Features.Orders
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Infrastructure.Services;

    public class GuestOrderService :  OrderService, IGuestOrderService
    {
        public GuestOrderService(NutriBestDbContext db,
            ICurrentUserService currentUserService,
            IConfiguration config)
            :base(db, currentUserService, config)
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
