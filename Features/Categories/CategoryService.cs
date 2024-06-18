﻿namespace NutriBest.Server.Features.Categories
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Categories.Models;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;

    public class CategoryService : ICategoryService, ITransientService
    {
        private readonly NutriBestDbContext db;

        public CategoryService(NutriBestDbContext db)
            => this.db = db;

        public async Task<int> Create(string name)
        {
            if (await db.Categories.AnyAsync(x => x.Name == name))
                throw new InvalidOperationException("Category with this name already exists!");

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

        public async Task<bool> Remove(string name)
        {
            var category = await db.Categories
                .FirstOrDefaultAsync(x => x.Name == name);

            if (category == null)
                throw new ArgumentNullException("Invalid brand!");

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
        {
            var categoriesCount = await db.Categories
                .Select(x => new CategoryCountServiceModel
                {
                    Category = x.Name,
                    Count = x.ProductsCategories
                    .Where(y => y.CategoryId == x.Id).Count(),
                })
                .ToListAsync();

            return categoriesCount;
        }
    }
}
