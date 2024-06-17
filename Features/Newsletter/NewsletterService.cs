namespace NutriBest.Server.Features.Newsletter
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Email;
    using NutriBest.Server.Features.Email.Models;
    using NutriBest.Server.Features.Newsletter.Extensions;
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
            if (await db.Newsletter.AnyAsync(x => x.Email == email && !x.IsDeleted))
            {
                throw new InvalidOperationException($"'{email}' is already subscribed!");
            }

            var (isAnonymous, ordersCount, name, phoneNumber) = await this.GetNewsletterData(db, userManager, email);

            var subscription = new Newsletter
            {
                IsAnonymous = isAnonymous,
                Email = email,
                HasOrders = ordersCount != 0,
                TotalOrders = ordersCount,
                Name = name,
                PhoneNumber = phoneNumber
            };

            db.Newsletter.Add(subscription);

            await db.SaveChangesAsync();

            return subscription.Id;
        }

        public async Task<AllSubscribersServiceModel> AllSubscribers(int page, string? search, string? groupType)
        {
            var totalSubscribers = await db.Newsletter
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
            var newsletter = await db.Newsletter
                .FirstOrDefaultAsync(x => x.Email == email && !x.IsDeleted);

            if (newsletter == null)
                return false;

            db.Newsletter.Remove(newsletter);

            await db.SaveChangesAsync();

            return true;
        }

        private async Task<List<SubscriberServiceModel>> GetFilteredSubscribers(int page, string? search, string? groupType)
        {
            var subscribers = db.Newsletter
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

                await CheckGroupType(subscriberModel, subscribersToReturn, groupType);
            }


            subscribersToReturn = subscribersToReturn
                .Skip((page - 1) * UsersPerPage)
                .Take(UsersPerPage)
                .ToList();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                subscribersToReturn = subscribersToReturn
                    .Where(x => (x.Name != null && x.Name.ToLower().Contains(search)) ||
                    (x.PhoneNumber != null && x.PhoneNumber.ToLower().Contains(search)) ||
                    x.Email.ToLower().Contains(search))
                    .ToList();
            }

            return subscribersToReturn;
        }

        private async Task CheckGroupType(SubscriberServiceModel subscriberModel, List<SubscriberServiceModel> subscribersToReturn, string? groupType)
        {
            switch (groupType)
            {
                case "withOrders":
                    if (subscriberModel.IsAnonymous)
                    {
                        var guestOrder = await db.GuestsOrders
                            .FirstOrDefaultAsync(x => x.Email == subscriberModel.Email);

                        if (guestOrder != null)
                        {
                            subscribersToReturn.Add(subscriberModel);
                        }
                    }
                    else
                    {
                        var userOrder = await db.UsersOrders
                           .FirstOrDefaultAsync(x => x.CreatedBy == subscriberModel.Email);

                        if (userOrder != null)
                        {
                            subscribersToReturn.Add(subscriberModel);
                        }
                    }
                    break;
                case "withoutOrders":
                    if (subscriberModel.IsAnonymous)
                    {
                        var guestOrder = await db.GuestsOrders
                            .FirstOrDefaultAsync(x => x.Email == subscriberModel.Email);

                        if (guestOrder == null)
                        {
                            subscribersToReturn.Add(subscriberModel);
                        }
                    }
                    else
                    {
                        var userOrder = await db.UsersOrders
                           .FirstOrDefaultAsync(x => x.CreatedBy == subscriberModel.Email);

                        if (userOrder == null)
                        {
                            subscribersToReturn.Add(subscriberModel);
                        }
                    }
                    break;
                case "guests":
                    if (subscriberModel.IsAnonymous)
                        subscribersToReturn.Add(subscriberModel);
                    break;
                case "users":
                    if (!subscriberModel.IsAnonymous)
                        subscribersToReturn.Add(subscriberModel);
                    break;
                default:
                    subscribersToReturn.Add(subscriberModel);
                    break;
            }
        }
    }
}
