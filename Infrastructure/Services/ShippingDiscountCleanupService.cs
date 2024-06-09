using Microsoft.EntityFrameworkCore;
using NutriBest.Server.Data;

namespace NutriBest.Server.Infrastructure.Services
{
    public class ShippingDiscountCleanupService : BackgroundService
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
                    var dbContext = scope.ServiceProvider.GetRequiredService<NutriBestDbContext>();

                    var expiredShippingDiscounts = await dbContext.ShippingDiscounts
                        .Where(x => !x.IsDeleted)
                        .Where(x => x.EndDate >= DateTime.UtcNow)
                        .ToListAsync(stoppingToken);

                    foreach (var shippingDiscount in expiredShippingDiscounts)
                    {
                        var country = await dbContext.Countries
                            .FirstOrDefaultAsync(x => x.ShippingDiscountId == shippingDiscount.Id);

                        if (country != null)
                        {
                            country.ShippingDiscountId = null;
                            dbContext.ShippingDiscounts.Remove(shippingDiscount);
                        }
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(interval, stoppingToken);
            }
        }
    }
}
