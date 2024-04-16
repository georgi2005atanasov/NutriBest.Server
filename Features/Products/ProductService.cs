﻿namespace NutriBest.Server.Features.Products
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Products.Extensions;
    using NutriBest.Server.Features.Products.Models;
    using static ServicesConstants.PaginationConstants; // make separate constants class

    public class ProductService : IProductService
    {
        private readonly NutriBestDbContext db;

        public ProductService(NutriBestDbContext db)
            => this.db = db;

        public async Task<AllProductsModel> All(int page,
            string? categoriesFilter,
            string? priceFilter)
        {
            var query = db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(categoriesFilter))
            {
                var categoriesToCheck = categoriesFilter
                    .Split()
                    .ToList();

                query = query
                    .Where(x => x.ProductsCategories
                                .Select(x => x.Category.Name)
                                .Any(x => categoriesToCheck.Contains(x)));
            }

            int pagesToSkip = (page - 1) * productsPerPage;

            var queryProducts = query.OrderBy(x => x.CreatedOn)
                         .Select(x => new ProductListingModel
                         {
                             ProductId = x.ProductId,
                             Name = x.Name,
                             Price = x.Price,
                             ProductImage = new ImageListingModel
                             {
                                 ImageData = x.ProductImage.ImageData,
                                 ContentType = x.ProductImage.ContentType
                             },
                             Categories = x.ProductsCategories
                             .Select(c => c.Category.Name)
                             .ToList()
                         })
                         .AsQueryable();

            var productsCount = queryProducts.Count();

            if (!string.IsNullOrEmpty(priceFilter))
            {
                if (priceFilter == "desc")
                    queryProducts = queryProducts
                        .OrderByDescending(x => x.Price);
                else if (priceFilter == "asc")
                    queryProducts = queryProducts
                        .OrderBy(x => x.Price);
            }

            var products = await queryProducts.ToListAsync();

            var productsRows = this.GetProductsRows(products);

            return new AllProductsModel
            {
                ProductsRows = productsRows,
                Count = productsCount,
            };
        }

        public async Task<int> Create(string name,
            string description,
            double price,
            List<int> categoriesIds,
            string imageData,
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
                    product.ProductsCategories
                        .Add(new ProductCategory { CategoryId = id });
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
                return null;

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

        public async Task<int> Update(int productId,
            string name,
            string description,
            double price,
            List<int> categoriesIds,
            string imageData,
            string contentType)
        {
            var product = await db.Products
                .FirstAsync(x => x.ProductId == productId);

            var productImage = new ProductImage
            {
                ImageData = imageData,
                ContentType = contentType
            };

            product.Name = name;
            product.Description = description;
            product.Price = price;
            product.ProductImage = productImage;
            product.ProductsCategories = new List<ProductCategory>();

            var existingMappings = db.ProductsCategories.Where(pc => pc.ProductId == productId);
            db.ProductsCategories.RemoveRange(existingMappings);

            foreach (var id in categoriesIds)
            {
                product.ProductsCategories
                    .Add(new ProductCategory { CategoryId = id });
            }

            db.Products.Update(product);
            await db.SaveChangesAsync();

            return productId;
        }

        public async Task<bool> Delete(int productId)
        {
            try
            {

                var product = await db.Products
                    .FirstAsync(x => x.ProductId == productId);

                var productsCategories = await db.ProductsCategories
                    .Where(x => x.ProductId == productId)
                    .ToListAsync();

                var productImage = await db.ProductsImages
                    .FirstAsync(x => x.ProductImageId == product.ProductImageId);

                db.ProductsCategories.RemoveRange(productsCategories);
                db.Products.Remove(product);
                db.ProductsImages.Remove(productImage);

                await db.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
