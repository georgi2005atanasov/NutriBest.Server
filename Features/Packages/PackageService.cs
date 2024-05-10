namespace NutriBest.Server.Features.Packages
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;

    public class PackageService : IPackageService
    {
        private readonly NutriBestDbContext db;

        public PackageService(NutriBestDbContext db)
            => this.db = db;

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
                throw new ArgumentNullException("Invalid package!");

            await db.ProductsPackagesFlavours.ForEachAsync(x =>
            {
                if (x.PackageId == package.Id)
                {
                    x.IsDeleted = true;
                }
            });

            db.Packages.Remove(package);

            await db.SaveChangesAsync();

            return true;
        }
    }
}
