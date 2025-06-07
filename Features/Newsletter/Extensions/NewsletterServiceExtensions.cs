namespace NutriBest.Server.Features.Newsletter.Extensions
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;

    public static class NewsletterServiceExtensions
    {
        public static async Task<(bool isAnonymous, int ordersCount, string name, string phoneNumber)>
            GetNewsletterData(this INewsletterService service, NutriBestDbContext db, UserManager<User> userManager, string email)
        {
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
                    .Where(x => x.CreatedBy == user.UserName)
                    .CountAsync();

                var profile = await db.Profiles
                    .FirstOrDefaultAsync(x => x.UserId == user.Id);

                name = profile != null ? profile.Name ?? "" : "";
                phoneNumber = user.PhoneNumber;
            }

            return (isAnonymous, ordersCount, name, phoneNumber);
        }
    }
}
