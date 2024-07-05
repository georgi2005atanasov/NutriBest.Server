using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Categories
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Categories.Models;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;
    using static ErrorMessages.CategoriesController;

    public class CategoryService : ICategoryService, ITransientService
    {
        private readonly NutriBestDbContext db;

        public CategoryService(NutriBestDbContext db)
            => this.db = db;

        public async Task<int> Create(string name)
        {
            if (await db.Categories.AnyAsync(x => x.Name == name))
                throw new InvalidOperationException(CategoryAlreadyExists);

            var category = new Category
            {
                Name = name
            };

            db.Categories.Add(category);

            await db.SaveChangesAsync();

            return category.Id;
        }

        public async Task<List<int>> GetCategoriesIds(List<string> categories)
        {
            var allCategories = await db.Categories
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                })
                .OrderBy(x => x.Name)
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

        public async Task<bool> Remove(string name)
        {
            var category = await db.Categories
                .FirstOrDefaultAsync(x => x.Name == name);

            if (category == null)
                throw new ArgumentNullException(InvalidCategory);

            var productsToDelete = db.Products
                .Where(x => x.ProductsCategories
                .Any(x => x.CategoryId == category.Id))
                .AsQueryable();

            var promotionsToDelete = db.Promotions
                .Where(x => x.Category == category.Name)
                .AsQueryable();

            var productsCategories = db.ProductsCategories
                .Where(x => x.CategoryId == category.Id)
                .AsQueryable();

            db.Products.RemoveRange(productsToDelete);
            db.Promotions.RemoveRange(promotionsToDelete);
            db.ProductsCategories.RemoveRange(productsCategories);
            db.Categories.Remove(category);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<CategoryCountServiceModel>> GetProductsCountByCategory()
        => await db.Categories
                .Select(x => new CategoryCountServiceModel
                {
                    Category = x.Name,
                    Count = x.ProductsCategories
                    .Where(y => y.CategoryId == x.Id).Count(),
                })
                .OrderBy(x => x.Category)
                .ToListAsync();

        public async Task<List<CategoryServiceModel>> All()
            => await db.Categories
                    .Select(x => new CategoryServiceModel
                    {
                        Name = x.Name
                    })
                    .OrderBy(x => x.Name)
                    .ToListAsync();
    }
}
