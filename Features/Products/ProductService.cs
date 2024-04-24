namespace NutriBest.Server.Features.Products
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Images.Models;
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
            string? priceFilter,
            string? alphaFilter,
            string? productsView,
            string? search,
            string? priceRange)
        {
            var query = db.Products.AsQueryable();

            var maxPrice = (int)Math.Ceiling(query.Select(x => x.Price).Max());

            query = this.SelectByCategories(query, categoriesFilter ?? "");
            query = this.GetBySearch(query, search ?? "");
            query = this.GetByPriceRange(query, priceRange ?? "");

            int pagesToSkip = (page - 1) * ((productsView == "all") ? productsPerPage : productsPerTable);

            var queryProducts = query.OrderBy(x => x.CreatedOn)
                         .Where(x => !x.IsDeleted)
                         .Select(x => new ProductListingModel
                         {
                             ProductId = x.ProductId,
                             Name = x.Name,
                             Price = x.Price,
                             Categories = x.ProductsCategories
                             .Select(c => c.Category.Name)
                             .ToList(),
                             Quantity = x.Quantity
                         })
                         .AsQueryable();


            queryProducts = this.OrderByName(queryProducts, alphaFilter ?? "");

            var productsCount = queryProducts.Count();

            queryProducts = this.OrderByPrice(queryProducts, priceFilter ?? "");


            queryProducts = queryProducts
                .Skip(pagesToSkip)
                .Take((productsView == "all") ? productsPerPage : productsPerTable);

            var products = await queryProducts.ToListAsync();

            var productsRows = this.GetProductsRows(products,
                (productsView == "all") ? productsPerRow : productsPerRowInTable);

            return new AllProductsModel
            {
                ProductsRows = productsRows,
                Count = productsCount,
                MaxPrice = maxPrice
            };
        }

        public async Task<int> Create(string name,
            string description,
            decimal price,
            int? quantity,
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
                CreatedOn = DateTime.Now,
                Quantity = quantity
            };

            foreach (var id in categoriesIds)
            {
                if (!product.ProductsCategories.Any(x => x.CategoryId == id))
                    product.ProductsCategories
                        .Add(new ProductCategory
                        {
                            CategoryId = id
                        });
            }


            db.Products.Add(product);
            await db.SaveChangesAsync();

            return product.ProductId;
        }

        public async Task<ProductDetailsModel?> GetById(int id)
        {
            var product = await db.Products
                         .Where(x => !x.IsDeleted)
                         .Select(x => new ProductDetailsModel
                         {
                             ProductId = x.ProductId,
                             Name = x.Name,
                             Price = x.Price,
                             Categories = x.ProductsCategories
                             .Select(c => c.Category.Name)
                             .ToList(),
                             Description = x.Description,
                             Image = new ImageListingModel
                             {
                                 ContentType = x.ProductImage.ContentType,
                                 ImageData = x.ProductImage.ImageData
                             },
                             Quantity = x.Quantity,
                         })
                         .FirstOrDefaultAsync(x => x.ProductId == id);

            if (product == null)
                return null;

            return product;
        }

        public async Task<int> Update(int productId,
            string name,
            string description,
            decimal price,
            int? quantity,
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
            product.Quantity = quantity;

            var existingMappings = db.ProductsCategories
                .Where(pc => pc.ProductId == productId);

            db.ProductsCategories.RemoveRange(existingMappings);

            foreach (var id in categoriesIds)
            {
                product.ProductsCategories
                    .Add(new ProductCategory { CategoryId = id });
            }

            //handle productCategories, since they are not really being deleted.
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

                var productImage = await db.ProductsImages
                    .FirstAsync(x => x.ProductImageId == product.ProductImageId);

                db.Products.Remove(product);
                await db.ProductsCategories
                    .Where(x => x.ProductId == productId)
                    .ForEachAsync(pc =>
                    {
                        if (pc.ProductId == productId)
                        {
                            pc.IsDeleted = true;
                        }
                    });
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
