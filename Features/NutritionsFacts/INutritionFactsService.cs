namespace NutriBest.Server.Features.NutritionsFacts
{
    using NutriBest.Server.Features.NutritionsFacts.Models;

    public interface INutritionFactsService
    {
        Task<NutritionFactsServiceModel> Get(int productId);

        Task Add(int productId,
            string? proteins,
            string? sugar,
            string? carbohydrates,
            string? fats,
            string? saturatedFats,
            string? energyValue,
            string? salt);
    }
}
