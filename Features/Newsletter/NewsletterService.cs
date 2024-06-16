namespace NutriBest.Server.Features.Newsletter
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Email;
    using NutriBest.Server.Features.Email.Models;
    using NutriBest.Server.Features.Newsletter.Models;
    using static ServicesConstants.PaginationConstants;

    public class NewsletterService : INewsletterService
    {
        private readonly NutriBestDbContext db;
        private readonly UserManager<User> userManager;
        private readonly IEmailService emailService;

        public NewsletterService(NutriBestDbContext db,
            UserManager<User> userManager,
            IEmailService emailService)
        {
            this.db = db;
            this.userManager = userManager;
            this.emailService = emailService;
        }

        public async Task<int> Add(string email)
        {
            if (await db.Newsletters.AnyAsync(x => x.Email == email && !x.IsDeleted))
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

            //await emailService.SendJoinedToNewsletter(new EmailModel
            //{
            //    To = email,
            //    Subject = "Newsletter Subscription"
            //});

            return subscription.Id;
        }

        public async Task<AllSubscribersServiceModel> AllSubscribers(int page, string? search, string? groupType)
        {
            var totalSubscribers = await db.Newsletters
                .CountAsync();

            var allSubscribersModel = new AllSubscribersServiceModel
            {
                TotalSubscribers = totalSubscribers,
                Subscribers = await GetFilteredSubscribers(page, search, groupType)
            };

            return allSubscribersModel;
        }

        public async Task<bool> Remove(string email)
        {
            var newsletter = await db.Newsletters
                .FirstOrDefaultAsync(x => x.Email == email && !x.IsDeleted);

            if (newsletter == null)
                return false;

            db.Newsletters.Remove(newsletter);

            await db.SaveChangesAsync();

            return true;
        }

        private async Task<List<SubscriberServiceModel>> GetFilteredSubscribers(int page, string? search, string? groupType)
        {
            var subscribers = db.Newsletters
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            var subscribersToReturn = new List<SubscriberServiceModel>();

            foreach (var subscriber in subscribers)
            {
                var subscriberModel = new SubscriberServiceModel
                {
                    Email = subscriber.Email,
                    HasOrders = subscriber.HasOrders,
                    TotalOrders = subscriber.TotalOrders,
                    IsAnonymous = subscriber.IsAnonymous,
                    Name = subscriber.Name,
                    PhoneNumber = subscriber.PhoneNumber,
                    RegisteredOn = subscriber.CreatedOn
                };

                switch (groupType)
                {
                    case "withOrders":
                        if (subscriber.IsAnonymous)
                        {
                            var guestOrder = await db.GuestsOrders
                                .FirstOrDefaultAsync(x => x.Email == subscriber.Email);

                            if (guestOrder != null)
                            {
                                subscribersToReturn.Add(subscriberModel);
                            }
                        }
                        else
                        {
                            var userOrder = await db.UsersOrders
                               .FirstOrDefaultAsync(x => subscriber.Email == subscriber.Email);

                            if (userOrder != null)
                            {
                                subscribersToReturn.Add(subscriberModel);
                            }
                        }
                        break;
                    case "withoutOrders":
                        if (subscriber.IsAnonymous)
                        {
                            var guestOrder = await db.GuestsOrders
                                .FirstOrDefaultAsync(x => x.Email == subscriber.Email);

                            if (guestOrder == null)
                            {
                                subscribersToReturn.Add(subscriberModel);
                            }
                        }
                        else
                        {
                            var userOrder = await db.UsersOrders
                               .FirstOrDefaultAsync(x => subscriber.Email == subscriber.Email);

                            if (userOrder == null)
                            {
                                subscribersToReturn.Add(subscriberModel);
                            }
                        }
                        break;
                    case "guests":
                        if (subscriber.IsAnonymous)
                            subscribersToReturn.Add(subscriberModel);
                        break;
                    case "users":
                        if (!subscriber.IsAnonymous)
                            subscribersToReturn.Add(subscriberModel);
                        break;
                    default:
                        subscribersToReturn.Add(subscriberModel);
                        break;
                }
            }

            subscribersToReturn = subscribersToReturn
                .Skip((page - 1) * UsersPerPage)
                .Take(UsersPerPage)
                .ToList();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                subscribersToReturn = subscribersToReturn
                    .Where(x => x.Name!.ToLower().Contains(search) ||
                    x.PhoneNumber!.ToLower().Contains(search) ||
                    x.Email.ToLower().Contains(search))
                    .ToList();
            }

            return subscribersToReturn;
        }
    }
}
