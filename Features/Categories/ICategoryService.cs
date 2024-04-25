using NutriBest.Server.Features.Categories.Models;

namespace NutriBest.Server.Features.Categories
{
    public interface ICategoryService
    {
        Task<List<int>> GetCategoriesIds(List<string> categories);

        Task<IEnumerable<CategoryCountServiceModel>> GetProductsCountByCategory();
    }
}
