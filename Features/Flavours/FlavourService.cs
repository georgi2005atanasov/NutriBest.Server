namespace NutriBest.Server.Features.Flavours
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Flavours.Models;
    using NutriBest.Server.Features.Products;
    using NutriBest.Server.Infrastructure.Services;
    using System.Collections.Generic;

    public class FlavourService : IFlavourService
    {
        private readonly NutriBestDbContext db;
        private readonly IProductService productService;
        private readonly ICurrentUserService currentUserService;

        public FlavourService(NutriBestDbContext db,
            IProductService productService,
            ICurrentUserService currentUserService)
        {
            this.db = db;
            this.productService = productService;
            this.currentUserService = currentUserService;
        }

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
        {
            var products = await db.ProductsPackagesFlavours
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
                .ToListAsync();

            return products;
        }
    }
}
