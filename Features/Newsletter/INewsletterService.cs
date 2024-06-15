namespace NutriBest.Server.Features.Newsletter
{
    public interface INewsletterService
    {
        public Task<int> Add(string email);
    }
}
