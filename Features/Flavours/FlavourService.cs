using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Flavours
{
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Flavours.Models;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;
    using static ErrorMessages.FlavoursController;

    public class FlavourService : IFlavourService, ITransientService
    {
        private readonly NutriBestDbContext db;

        public FlavourService(NutriBestDbContext db)
        {
            this.db = db;
        }

        public async Task<int> Create(string name)
        {
            if (await db.Flavours.AnyAsync(x => x.FlavourName == name))
                throw new InvalidOperationException(FlavourAlreadyExists);

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
                throw new ArgumentNullException(InvalidFlavour);

            var productsPackagesFlavours = await db.ProductsPackagesFlavours
                .Where(x => x.FlavourId == flavour.Id)
                .ToListAsync();

            productsPackagesFlavours.ForEach(x =>
            {
                if (x.FlavourId == flavour.Id)
                {
                    x.IsDeleted = true;
                }
            });

            flavour.IsDeleted = true;

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<List<FlavourCountServiceModel>> GetProductsByFlavourCount()
            => await db.ProductsPackagesFlavours
                .GroupBy(ppf => ppf.FlavourId)
                .Select(g => new FlavourCountServiceModel
                {
                    Name = db.Flavours
                              .Where(p => p.Id == g.Key)
                              .Select(p => p.FlavourName)
                              .FirstOrDefault() ?? "",
                    Count = g.Select(ppf => ppf.ProductId)
                                .Distinct()
                                .Count()
                })
                .OrderBy(x => x.Name)
                .ToListAsync();

        public async Task<List<FlavourServiceModel>> All()
            => await db.Flavours
                    .Select(x => new FlavourServiceModel
                    {
                        Name = x.FlavourName
                    })
                    .OrderBy(x => x.Name)
                    .ToListAsync();
    }
}
