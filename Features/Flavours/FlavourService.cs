namespace NutriBest.Server.Features.Flavours
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;

    public class FlavourService : IFlavourService
    {
        private readonly NutriBestDbContext db;

        public FlavourService(NutriBestDbContext db)
            => this.db = db;

        public async Task<int> Create(string name)
        {
            if (await db.Flavours.AnyAsync(x => x.FlavourName == name))
                throw new InvalidOperationException("Flavour with this name already exists!");

            var flavour = new Flavour
            {
                FlavourName = name
            };

            db.Flavours.Add(flavour);

            await db.SaveChangesAsync();

            return flavour.Id;
        }

        public async Task<bool> Remove(string name)
        {
            var flavour = await db.Flavours
                .FirstOrDefaultAsync(x => x.FlavourName == name);

            if (flavour == null)
                throw new ArgumentNullException("Invalid flavour!");

            await db.ProductsPackagesFlavours.ForEachAsync(x =>
            {
                if (x.FlavourId == flavour.Id)
                {
                    x.IsDeleted = true;
                }
            });

            db.Flavours.Remove(flavour);

            await db.SaveChangesAsync();

            return true;
        }
    }
}
