namespace NutriBest.Server.Features.Newsletter
{
    using NutriBest.Server.Features.Newsletter.Models;

    public interface INewsletterService
    {
        public Task<int> Add(string email);

        public Task<bool> Remove(string email);

        public Task<AllSubscribersServiceModel> AllSubscribers(int page,
            string? search,
            string? groupType);

        public Task<List<SubscriberServiceModel>> AllExportSubscribers(string? search,
            string? groupType);
    }
}
