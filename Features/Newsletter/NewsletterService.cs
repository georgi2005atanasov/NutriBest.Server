using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Newsletter
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Notifications;
    using NutriBest.Server.Features.Newsletter.Models;
    using NutriBest.Server.Features.Newsletter.Extensions;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;
    using static ErrorMessages.NewsletterController;
    using static SuccessMessages.NotificationService;
    using static ServicesConstants.PaginationConstants;

    public class NewsletterService : INewsletterService, ITransientService
    {
        private readonly NutriBestDbContext db;
        private readonly UserManager<User> userManager;
        private readonly INotificationService notificationService;

        public NewsletterService(NutriBestDbContext db,
            UserManager<User> userManager,
            INotificationService notificationService)
        {
            this.db = db;
            this.userManager = userManager;
            this.notificationService = notificationService;
        }

        public async Task<int> Add(string email)
        {
            if (await db.Newsletter.AnyAsync(x => x.Email == email && !x.IsDeleted))
                throw new InvalidOperationException(string.Format(UserIsAlreadySubscribed, email));

            var (isAnonymous, ordersCount, name, phoneNumber) = await this
                .GetNewsletterData(db, userManager, email);

            if (db.Newsletter
                .IgnoreQueryFilters()
                .Any(x => x.Email == email && x.IsDeleted))
            {
                var subscriber = await db.Newsletter
                    .IgnoreQueryFilters()
                    .FirstAsync(x => x.Email == email);

                subscriber.IsDeleted = false;
                subscriber.DeletedOn = null;
                subscriber.DeletedBy = null;
                subscriber.TotalOrders = ordersCount;
                subscriber.HasOrders = ordersCount > 0;
                subscriber.PhoneNumber = phoneNumber;
                subscriber.Name = name;
                await db.SaveChangesAsync();
                return subscriber.Id;
            }

            var subscription = new Newsletter
            {
                IsAnonymous = isAnonymous,
                Email = email,
                HasOrders = ordersCount != 0,
                TotalOrders = ordersCount,
                Name = name,
                PhoneNumber = phoneNumber,
                VerificationToken = Guid.NewGuid().ToString()
            };

            db.Newsletter.Add(subscription);

            // can be stopped anytime
            await notificationService.SendNotificationToAdmin("success", string.Format(UserSignedUpForNewsletter, email));

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

        public async Task<List<SubscriberServiceModel>> AllExportSubscribers(string? search, string? groupType)
        {
            var allSubscribers = new List<SubscriberServiceModel>();

            int currentPage = 1;
            while (true)
            {
                var data = await AllSubscribers(currentPage, search, groupType);

                if (!data.Subscribers.Any())
                {
                    return allSubscribers;
                }

                foreach (var subscriber in data.Subscribers)
                {
                    allSubscribers.Add(subscriber);
                }

                currentPage++;
            }
        }

        public async Task<bool> RemoveForAdmin(string email)
        {
            var newsletter = await db.Newsletter
                .FirstOrDefaultAsync(x => x.Email == email && !x.IsDeleted);

            if (newsletter == null)
                return false;

            db.Newsletter.Remove(newsletter);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Unsubscribe(string email, string token)
        {
            var newsletter = await db.Newsletter
                .FirstOrDefaultAsync(x => x.Email == email &&
                token == x.VerificationToken &&
                !x.IsDeleted);

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
                var (isAnonymous, ordersCount, name, phoneNumber) = await this.GetNewsletterData(db, userManager, subscriber.Email);

                if (subscriber.TotalOrders != ordersCount)
                {
                    subscriber.TotalOrders = ordersCount;
                    subscriber.HasOrders = true;
                }

                var subscriberModel = new SubscriberServiceModel
                {
                    Email = subscriber.Email,
                    HasOrders = ordersCount != 0,
                    TotalOrders = ordersCount,
                    IsAnonymous = isAnonymous,
                    Name = name,
                    PhoneNumber = phoneNumber,
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
                    (x.PhoneNumber != null &&
                        (x.PhoneNumber.ToLower().StartsWith(search) ||
                         x.PhoneNumber.ToLower().EndsWith(search))) ||
                    x.Email.ToLower().Contains(search))
                    .ToList();
            }

            subscribersToReturn = subscribersToReturn
                .OrderByDescending(x => x.RegisteredOn)
                .ToList();

            await db.SaveChangesAsync();

            return subscribersToReturn;
        }

        private async Task CheckGroupType(SubscriberServiceModel subscriberModel, List<SubscriberServiceModel> subscribersToReturn, string? groupType)
        {
            await Task.Run(() =>
            {
                switch (groupType)
                {
                    case "withOrders":
                        if (subscriberModel.HasOrders)
                        {
                            subscribersToReturn.Add(subscriberModel);
                        }
                        break;
                    case "withoutOrders":
                        if (!subscriberModel.HasOrders)
                        {
                            subscribersToReturn.Add(subscriberModel);
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
            });
        }
    }
}
