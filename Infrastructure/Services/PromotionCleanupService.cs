namespace NutriBest.Server.Infrastructure.Services
{
    using NutriBest.Server.Data;

    public class PromotionCleanupService : BackgroundService, Infrastructure.Extensions.ServicesInterfaces.IHostedService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly TimeSpan interval = TimeSpan.FromHours(1);

        public PromotionCleanupService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<NutriBestDbContext>();
                    var now = DateTime.UtcNow;

                    var expiredPromotions = db.Promotions
                        .Where(p => p.EndDate != null && p.EndDate <= now && p.IsActive)
                        .ToList();

                    foreach (var promotion in expiredPromotions)
                    {
                        promotion.IsActive = false;
                    }
                    db.Promotions.RemoveRange(expiredPromotions);

                    await db.SaveChangesAsync();
                }

                await Task.Delay(interval, stoppingToken);
            }
        }
    }
}