namespace NutriBest.Server.Infrastructure.Services
{
    using NutriBest.Server.Data;

    public class PromotionActivationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromHours(1);

        public PromotionActivationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<NutriBestDbContext>();
                    var now = DateTime.UtcNow;

                    var promotionsToActivate = dbContext.Promotions
                        .Where(p => p.StartDate <= now && !p.IsActive)
                        .ToList();

                    foreach (var promotion in promotionsToActivate)
                    {
                        promotion.IsActive = true;
                    }

                    await dbContext.SaveChangesAsync();
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}