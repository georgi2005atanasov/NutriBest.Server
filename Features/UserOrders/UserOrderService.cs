namespace NutriBest.Server.Features.UserOrders
{
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Orders;
    using NutriBest.Server.Infrastructure.Services;
    using System.Threading.Tasks;

    public class UserOrderService : OrderService, IUserOrderService
    {
        public UserOrderService(NutriBestDbContext db,
            ICurrentUserService currentUserService)
            :base(db, currentUserService)
        {
        }

        public async Task<int> CreateUserOrder(string userId, int orderId, string name, string email, string? phoneNumber)
        {
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
