namespace NutriBest.Server.Infrastructure.Services
{
    using NutriBest.Server.Data;

    public class PromotionActivationService : BackgroundService, Infrastructure.Extensions.ServicesInterfaces.IHostedService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly TimeSpan interval = TimeSpan.FromHours(1);

        public PromotionActivationService(IServiceProvider serviceProvider)
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

                    var promotionsToActivate = dbContext.Promotions
                        .Where(p => p.StartDate <= now && !p.IsActive);

                    foreach (var promotion in promotionsToActivate)
                    {
                        promotion.IsActive = true;
                    }

                    await dbContext.SaveChangesAsync();
                }

                await Task.Delay(interval, stoppingToken);
            }
        }
    }
}