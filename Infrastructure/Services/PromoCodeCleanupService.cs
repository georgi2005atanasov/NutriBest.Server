namespace NutriBest.Server.Infrastructure.Services
{
    using NutriBest.Server.Data;
    using Microsoft.EntityFrameworkCore;

    public class PromoCodeCleanupService : BackgroundService, Infrastructure.Extensions.ServicesInterfaces.IHostedService
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

                    var tenDaysAgo = DateTime.UtcNow.AddDays(-10);

                    var expiredPromoCodes = await dbContext.PromoCodes
                        .Where(p => p.IsValid && !p.IsDeleted)
                        .Where(p => p.CreatedOn <= tenDaysAgo)
                        .ToListAsync(stoppingToken);

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
