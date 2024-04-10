namespace NutriBest.Server.Features.Categories
{
    public interface ICategoryService
    {
        Task<List<int>> GetCategoriesIds(List<string> categories);
    }
}
