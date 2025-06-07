namespace NutriBest.Server.Infrastructure.Services
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;

    public class ShippingDiscountCleanupService : BackgroundService, Infrastructure.Extensions.ServicesInterfaces.IHostedService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly TimeSpan interval = TimeSpan.FromHours(1);

        public ShippingDiscountCleanupService(IServiceProvider serviceProvider)
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

                    var expiredShippingDiscounts = await db.ShippingDiscounts
                        .Where(x => !x.IsDeleted)
                        .Where(x => x.EndDate <= DateTime.UtcNow)
                        .ToListAsync(stoppingToken);

                    foreach (var shippingDiscount in expiredShippingDiscounts)
                    {
                        var country = await db.Countries
                            .FirstOrDefaultAsync(x => x.ShippingDiscountId == shippingDiscount.Id);

                        if (country != null)
                        {
                            country.ShippingDiscountId = null;
                            db.ShippingDiscounts.Remove(shippingDiscount);
                        }
                    }

                    await db.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(interval, stoppingToken);
            }
        }
    }
}
