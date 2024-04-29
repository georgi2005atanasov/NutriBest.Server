namespace NutriBest.Server.Features.NutritionsFacts
{
    using NutriBest.Server.Features.NutritionsFacts.Models;

    public interface INutritionFactsService
    {
        Task<NutritionFactsServiceModel> Get(int productId);

        Task Add(int productId,
            double? proteins,
            double? sugar,
            double? carbohydrates,
            double? fats,
            double? saturatedFats,
            double? energyValue,
            double? salt);

        Task Remove(int productId);
    }
}
