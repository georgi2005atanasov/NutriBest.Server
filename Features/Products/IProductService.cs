﻿namespace NutriBest.Server.Features.Products
{
    using NutriBest.Server.Features.Products.Models;

    public interface IProductService
    {
        Task<int> Create(string name,
            string description,
            decimal price,
            int? quantity,
            List<int> categoriesIds,
            string imageData,
            string contentType);

        Task<AllProductsServiceModel> All(int page,
            string? categories,
            string? priceFilter,
            string? alphaFilter,
            string? productsView,
            string? search,
            string? priceRange);

        Task<ProductServiceModel> GetById(int id);

        Task<int> Update(int id,
            string name,
            string description,
            decimal price,
            int? quantity,
            List<int> categoriesIds,
            string imageData,
            string contentType);

        Task<bool> Delete(int productId);
    }
}
