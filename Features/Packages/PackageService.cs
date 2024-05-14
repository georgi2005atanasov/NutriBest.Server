namespace NutriBest.Server.Features.Packages
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Infrastructure.Services;

    public class PackageService : IPackageService
    {
        private readonly NutriBestDbContext db;
        private readonly ICurrentUserService currentUserService;

        public PackageService(NutriBestDbContext db,
            ICurrentUserService currentUserService)
        {
            this.db = db;
            this.currentUserService = currentUserService;
        }

        public async Task<int> Create(int grams)
        {
            if (await db.Packages.AnyAsync(x => x.Grams == grams))
                throw new InvalidOperationException("Package with these grams already exists!");

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
    }
}
