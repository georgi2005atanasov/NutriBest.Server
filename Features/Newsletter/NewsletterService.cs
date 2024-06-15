namespace NutriBest.Server.Features.Newsletter
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;

    public class NewsletterService : INewsletterService
    {
        private readonly NutriBestDbContext db;
        private readonly UserManager<User> userManager;

        public NewsletterService(NutriBestDbContext db,
            UserManager<User> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        public async Task<int> Add(string email)
        {
            if (await db.Newsletters.AnyAsync(x => x.Email == email))
            {
                throw new InvalidOperationException($"'{email}' is already subscribed!");
            }

            bool isAnonymous = false;
            int ordersCount = 0;
            string name = "";
            string phoneNumber = "";

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                isAnonymous = true;

                var guestOrders = db.GuestsOrders
                    .Where(x => x.Email == email)
                    .AsQueryable();

                if (await guestOrders.AnyAsync(x => !string.IsNullOrEmpty(x.PhoneNumber)))
                {
                    var orderWithNumber = await guestOrders
                        .LastAsync(x => x.PhoneNumber != null);

                    phoneNumber = orderWithNumber.PhoneNumber ?? "";
                }

                ordersCount = await guestOrders.CountAsync();
            }
            else
            {
                ordersCount = await db.UsersOrders
                    .Where(x => x.CreatedBy == email)
                    .CountAsync();

                var profile = await db.Profiles
                    .FirstAsync(x => x.UserId == user.Id);

                name = profile.Name ?? "";
                phoneNumber = user.PhoneNumber;
            }

            var subscription = new Newsletter
            {
                IsAnonymous = isAnonymous,
                Email = email,
                HasOrders = ordersCount != 0,
                TotalOrders = ordersCount,
                Name = name,
                PhoneNumber = phoneNumber
            };

            db.Newsletters.Add(subscription);

            await db.SaveChangesAsync();

            return subscription.Id;
        }
    }
}
