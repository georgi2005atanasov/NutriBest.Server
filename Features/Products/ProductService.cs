﻿namespace NutriBest.Server.Features.Products
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Products.Models;

    public class ProductService : IProductService
    {
        private readonly NutriBestDbContext db;

        public ProductService(NutriBestDbContext db)
            => this.db = db;

        public async Task<IEnumerable<ProductListingModel>> All()
            => await db.Products
                        .Select(x => new ProductListingModel
                        {
                            Id = x.ProductId,
                            Name = x.Name
                        })
                        .ToListAsync();

        public async Task<int> Create(string name,
            string description,
            decimal price,
            List<int> categoriesIds,
            byte[] imageData,
            string contentType)
        {
            var productImage = new ProductImage
            {
                ImageData = imageData,
                ContentType = contentType
            };

            var product = new Product
            {
                Name = name,
                Description = description,
                Price = price,
                ProductImage = productImage,
                ProductsCategories = new List<ProductCategory>(),
                CreatedOn = DateTime.Now
            };

            foreach (var id in categoriesIds)
            {
                if (!product.ProductsCategories.Any(x => x.CategoryId == id))
                {
                    product.ProductsCategories
                        .Add(new ProductCategory { CategoryId = id });
                }
            }


            db.Products.Add(product);
            await db.SaveChangesAsync();

            return product.ProductId;
        }

        public async Task<ProductDetailsModel?> GetById(int id)
        {
            var product = await db.Products
                .FirstOrDefaultAsync(x => x.ProductId == id);

            if (product == null)
            {
                return null;
            }

            var productDetailsModel = new ProductDetailsModel
            {
                ProductId = product.ProductId,
                Description = product.Description,
                Name = product.Name,
                Price = product.Price,
                ProductImageId = product.ProductImageId,
            };

            return productDetailsModel;
        }
    }
}
