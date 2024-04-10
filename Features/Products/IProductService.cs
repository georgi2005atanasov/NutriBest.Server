﻿using NutriBest.Server.Data.Models;
using NutriBest.Server.Features.Products.Models;

namespace NutriBest.Server.Features.Products
{
    public interface IProductService
    {
        Task<ProductImage> GetImage(IFormFile image, string contentType);
        Task<int> Create(string name, 
            string description, 
            decimal price,
            List<int> categoriesIds,
            byte[] imageData,
            string contentType);

        Task<IEnumerable<ProductListingModel>> All();

        Task<List<int>> GetCategoriesIds(List<string> categories);
    }
}
