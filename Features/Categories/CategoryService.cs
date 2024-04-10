namespace NutriBest.Server.Features.Categories
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;

    public class CategoryService : ICategoryService
    {
        private readonly NutriBestDbContext db;

        public CategoryService(NutriBestDbContext db) 
            => this.db = db;

        public async Task<List<int>> GetCategoriesIds(List<string> categories)
        {
            var allCategories = await db.Categories
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToListAsync();

            var categoriesIds = new List<int>();

            foreach (var category in categories)
            {
                var isCategory = allCategories.FirstOrDefault(x => x.Name == category);

                if (isCategory != null)
                {
                    var categoryToAdd = isCategory.Id;

                    categoriesIds.Add(categoryToAdd);
                }
            }

            return categoriesIds;
        }
    }
}
