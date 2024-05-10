﻿namespace NutriBest.Server.Features.Categories
{
    using NutriBest.Server.Features.Categories.Models;

    public interface ICategoryService
    {
        Task<List<int>> GetCategoriesIds(List<string> categories);

        Task<IEnumerable<CategoryCountServiceModel>> GetProductsCountByCategory();

        Task<int> Create(string name);

        Task<bool> Remove(string name);
    }
}
