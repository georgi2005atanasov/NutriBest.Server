namespace NutriBest.Server.Features.NutritionsFacts
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.NutritionsFacts.Models;

    public class NutritionFactsService : INutritionFactsService
    {
        private readonly NutriBestDbContext db;

        public NutritionFactsService(NutriBestDbContext db)
        {
            this.db = db;
        }

        public async Task Add(int productId,
            double? proteins,
            double? sugars,
            double? carbohydrates,
            double? fats,
            double? saturatedFats,
            double? energyValue,
            double? salt)
        {
            var details = await db.Products
                .Include(x => x.NutritionFacts)
                .FirstAsync(x => x.ProductId == productId);

            if (carbohydrates != null)
            {
                details.NutritionFacts.Carbohydrates = carbohydrates;
            }
            if (fats != null)
            {
                details.NutritionFacts.Fats = fats;
            }
            if (saturatedFats != null)
            {
                details.NutritionFacts.SaturatedFats = saturatedFats;
            }
            if (sugars != null)
            {
                details.NutritionFacts.Sugars = sugars;
            }
            if (proteins != null)
            {
                details.NutritionFacts.Proteins = proteins;
            }
            if (energyValue != null)
            {
                details.NutritionFacts.EnergyValue = energyValue;
            }
            if (salt != null)
            {
                details.NutritionFacts.Salt = salt;
            }

            await db.SaveChangesAsync();
        }

        public async Task<NutritionFactsServiceModel> Get(int productId, string name)
        {
            var product = await db.Products
                .Include(x => x.NutritionFacts)
                .FirstAsync(x => x.ProductId == productId);

            if (product.Name != name)
                throw new InvalidOperationException("Invalid product!");

            var facts = new NutritionFactsServiceModel
            {
                ProductId = productId,
                Carbohydrates = product.NutritionFacts.Carbohydrates,
                SaturatedFats = product.NutritionFacts.SaturatedFats,
                Fats = product.NutritionFacts.Fats,
                Sugars = product.NutritionFacts.Sugars,
                Proteins = product.NutritionFacts.Proteins,
                EnergyValue = product.NutritionFacts.EnergyValue,
                Salt = product.NutritionFacts.Salt,
            };

            return facts;
        }

        public async Task Remove(int productId, string name)
        {
            var product = await db.Products
                .FirstAsync(x => x.ProductId == productId);

            if (product.Name != name)
                throw new InvalidOperationException("Invalid product!");

            var details = await db.NutritionFacts
                .FirstAsync(x => x.ProductId == productId);

            details.Proteins = null;
            details.Sugars = null;
            details.Salt = null;
            details.Fats = null;
            details.SaturatedFats = null;
            details.Carbohydrates = null;
            details.EnergyValue = null;

            await db.SaveChangesAsync();
        }
    }
}
