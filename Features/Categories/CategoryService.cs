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
                .OrderBy(x => x.Id)
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

        public async Task<IEnumerable<CategoryCountModel>> GetProductsCountByCategory()
        {
            var categoriesCount = await db.Categories
                .Select(x => new CategoryCountModel
                {
                    Category = x.Name,
                    Count = x.ProductsCategories.Where(y => y.CategoryId == x.Id).Count(),
                })
                .ToListAsync();

            return categoriesCount;
        }
    }
}
