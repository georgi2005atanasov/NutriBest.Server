using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.NutritionsFacts
{
    using Microsoft.EntityFrameworkCore;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.NutritionsFacts.Models;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;
    using static ErrorMessages.NutritionFactsController;

    public class NutritionFactsService : INutritionFactsService, ITransientService
    {
        private readonly NutriBestDbContext db;
        private readonly IMapper mapper;

        public NutritionFactsService(NutriBestDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task Add(int productId,
            string? proteins,
            string? sugars,
            string? carbohydrates,
            string? fats,
            string? saturatedFats,
            string? energyValue,
            string? salt)
        {
            var details = await db.Products
                .Include(x => x.NutritionFacts)
                .FirstAsync(x => x.ProductId == productId);

            try
            {
                if (!string.IsNullOrEmpty(carbohydrates))
                    details.NutritionFacts.Carbohydrates = double.Parse(carbohydrates);

                if (!string.IsNullOrEmpty(fats))
                    details.NutritionFacts.Fats = double.Parse(fats);

                if (!string.IsNullOrEmpty(saturatedFats))
                    details.NutritionFacts.SaturatedFats = double.Parse(saturatedFats);

                if (!string.IsNullOrEmpty(sugars))
                    details.NutritionFacts.Sugars = double.Parse(sugars);

                if (!string.IsNullOrEmpty(proteins))
                    details.NutritionFacts.Proteins = double.Parse(proteins);

                if (!string.IsNullOrEmpty(energyValue))
                    details.NutritionFacts.EnergyValue = double.Parse(energyValue);

                if (!string.IsNullOrEmpty(salt))
                    details.NutritionFacts.Salt = double.Parse(salt);

                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new InvalidOperationException(InvalidNutritionFacts);
            }
        }

        public async Task<NutritionFactsServiceModel> Get(int productId)
        {
            var facts = await db.Products
                .Include(x => x.NutritionFacts)
                .Where(x => x.ProductId == productId)
                .ProjectTo<NutritionFactsServiceModel>(mapper.ConfigurationProvider)
                .FirstAsync();

            return facts;
        }

        public async Task<bool> Remove(int productId)
        {
            var product = await db.Products
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            var details = await db.NutritionFacts
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            if (details == null || product == null)
                throw new ArgumentNullException(InvalidProduct);

            details.Proteins = null;
            details.Sugars = null;
            details.Salt = null;
            details.Fats = null;
            details.SaturatedFats = null;
            details.Carbohydrates = null;
            details.EnergyValue = null;

            await db.SaveChangesAsync();

            return true;
        }
    }
}
