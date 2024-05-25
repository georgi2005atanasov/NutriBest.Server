namespace NutriBest.Server.Infrastructure.Services
{
    using NutriBest.Server.Data;

    public class PromoCodeCleanupService : BackgroundService
    {
        //this is background service that ensures the promo codes are valid within 10 days from their creation
        private readonly IServiceProvider serviceProvider;
        private readonly TimeSpan interval = TimeSpan.FromHours(1);

        public PromoCodeCleanupService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<NutriBestDbContext>();
                    var now = DateTime.UtcNow;

                    var expiredPromoCodes = dbContext.PromoCodes
                        .Where(p => (p.CreatedOn - DateTime.Now).Days > 10);

                    foreach (var promoCode in expiredPromoCodes)
                    {
                        promoCode.IsValid = false;
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(interval, stoppingToken);
            }
        }
    }
}
