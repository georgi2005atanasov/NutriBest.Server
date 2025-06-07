using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Packages
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Packages.Models;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;
    using static ErrorMessages.PackagesController;

    public class PackageService : IPackageService, ITransientService
    {
        private readonly NutriBestDbContext db;

        public PackageService(NutriBestDbContext db)
            => this.db = db;

        public async Task<List<PackageServiceModel>> All()
            => await db.Packages
                    .Select(x => new PackageServiceModel
                    {
                        Grams = x.Grams
                    })
                    .ToListAsync();

        public async Task<int> Create(int grams)
        {
            if (await db.Packages.AnyAsync(x => x.Grams == grams))
                throw new InvalidOperationException(PackageAlreadyExists);

            var package = new Package
            {
                Grams = grams
            };

            db.Packages.Add(package);

            await db.SaveChangesAsync();

            return package.Id;
        }

        public async Task<bool> Remove(int grams)
        {
            var package = await db.Packages
                .FirstOrDefaultAsync(x => x.Grams == grams);

            if (package == null)
                throw new ArgumentNullException("Invalid flavour!");

            var productsPackagesFlavours = await db.ProductsPackagesFlavours
                .Where(x => x.PackageId == package.Id)
                .ToListAsync();

            productsPackagesFlavours.ForEach(x =>
            {
                if (x.PackageId == package.Id)
                {
                    x.IsDeleted = true;
                }
            });

            package.IsDeleted = true;

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<List<PackageCountServiceModel>> GetProductsCountByQuantity()
            => await db.ProductsPackagesFlavours
                .GroupBy(ppf => ppf.PackageId)
                .Select(g => new PackageCountServiceModel
                {
                    Grams = db.Packages
                              .Where(p => p.Id == g.Key)
                              .Select(p => p.Grams)
                              .FirstOrDefault(),
                    Quantity = g.Select(ppf => ppf.ProductId)
                                .Distinct()
                                .Count()
                })
                .OrderBy(x => x.Grams)
                .ToListAsync();
    }
}
