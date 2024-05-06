namespace NutriBest.Server.Features.Products
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Images.Models;
    using NutriBest.Server.Features.Products.Extensions;
    using NutriBest.Server.Features.Products.Models;
    using NutriBest.Server.Features.Promotions;
    using static ServicesConstants.PaginationConstants; // make separate constants class

    public class ProductService : IProductService
    {
        private readonly NutriBestDbContext db;
        private readonly IPromotionService promotionService;
        private readonly IMapper mapper;

        public ProductService(NutriBestDbContext db,
            IPromotionService promotionService,
            IMapper mapper)
        {
            this.db = db;
            this.promotionService = promotionService;
            this.mapper = mapper;
        }

        public async Task<AllProductsServiceModel> All(int page,
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
            query = this.GetByPriceRangeWithPromotions(query, priceRange ?? "");

            var productsCount = query.Count();

            int pagesToSkip = (page - 1) * ((productsView == "all") ? productsPerPage : productsPerTable);

            var queryProducts = query
                .OrderByDescending(x => x.CreatedOn)
                         .Select(x => new ProductListingServiceModel
                         {
                             ProductId = x.ProductId,
                             Name = x.Name,
                             Price = x.Price,
                             Categories = x.ProductsCategories
                             .Select(c => c.Category.Name)
                             .ToList(),
                             Quantity = x.Quantity,
                             PromotionId = x.PromotionId,
                             Image = new ImageListingServiceModel
                             {
                                 ImageData = x.ProductImage.ImageData,
                                 ContentType = x.ProductImage.ContentType
                             }
                         })
                         .AsQueryable();

            queryProducts = this.OrderByName(queryProducts, alphaFilter ?? "");

            queryProducts = this.OrderByPrice(queryProducts, priceFilter ?? "");

            queryProducts = queryProducts
                .Skip(pagesToSkip)
                .Take((productsView == "all") ? productsPerPage : productsPerTable);

            var products = await Task.Run(() => queryProducts.ToList());

            foreach (var product in products)
            {
                if (product.PromotionId != null)
                {
                    try
                    {
                        var promotion = await promotionService.Get((int)product.PromotionId);

                        if (!promotion.IsActive)
                        {
                            throw new Exception();
                        }

                        if (promotion != null && promotion.DiscountPercentage != null)
                        {
                            product.DiscountPercentage = promotion.DiscountPercentage;
                        }

                        if (promotion != null && promotion.DiscountAmount != null)
                        {
                            product.DiscountPercentage = promotion.DiscountAmount * 100 / product.Price;
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            var productsRows = this.GetProductsRows(products,
                (productsView == "all") ? productsPerRow : productsPerRowInTable);

            return new AllProductsServiceModel
            {
                ProductsRows = productsRows,
                Count = productsCount,
                MaxPrice = maxPrice
            };
        }

        public async Task<int> Create(string name,
            string description,
            string brand,
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
                CreatedOn = DateTime.Now,
                Quantity = quantity
            };

            //add brand

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

        public async Task<ProductServiceModel> Get(int id)
        {
            var product = await db.Products
                         .Include(x => x.ProductDetails)
                         .ProjectTo<ProductServiceModel>(mapper.ConfigurationProvider)
                         .FirstOrDefaultAsync(x => x.ProductId == id);

            if (product == null)
                throw new ArgumentNullException("Invalid product!");

            return product;
        }

        public async Task<int> Update(int productId,
            string name,
            string description,
            string brand,
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

            //update the brand;

            var existingMappings = db.ProductsCategories
                .Where(pc => pc.ProductId == productId);

            db.ProductsCategories.RemoveRange(existingMappings);

            foreach (var id in categoriesIds)
            {
                product.ProductsCategories
                    .Add(new ProductCategory { CategoryId = id });
            }

            //handle productCategories, since they are not really being deleted.

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

                await db.ProductsDetails
                    .Where(x => x.ProductId == productId)
                    .ForEachAsync(pd =>
                    {
                        if (pd.ProductId == productId)
                            pd.IsDeleted = true;
                    });

                await db.NutritionFacts
                    .Where(x => x.ProductId == productId)
                    .ForEachAsync(nf =>
                    {
                        if (nf.ProductId == productId)
                            nf.IsDeleted = true;
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

        public async Task<ProductWithPromotionDetailsServiceModel> GetWithPromotion(int productId, int promotionId)
        {
            var (product, promotion) = await ValidateProductAndPromotionExistence(productId, promotionId);

            decimal newPrice = 0;

            if (promotion.DiscountPercentage != null)
                newPrice = product.Price * ((100 - (decimal)promotion.DiscountPercentage) / 100);

            if (promotion.DiscountAmount != null)
                newPrice = product.Price - (decimal)promotion.DiscountAmount;

            var productListingModel = await Get(productId);

            var productWithPromotion = new ProductWithPromotionDetailsServiceModel
            {
                Categories = productListingModel.Categories,
                Image = productListingModel.Image,
                Description = product.Description,
                Name = product.Name,
                Price = product.Price,
                PromotionId = product.PromotionId,
                ProductId = product.ProductId,
                Quantity = product.Quantity,
                DiscountAmount = promotion.DiscountAmount,
                DiscountPercentage = promotion.DiscountPercentage,
                NewPrice = newPrice
            };

            return productWithPromotion;
        }

        private async Task<(Product product, Promotion promotion)> ValidateProductAndPromotionExistence(int productId, int promotionId)
        {
            var promotion = await db.Promotions
                .FirstOrDefaultAsync(x => x.PromotionId == promotionId);

            var product = await db.Products
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            if (promotion == null || product == null)
                throw new ArgumentNullException("Invalid product!");

            if (product.PromotionId != promotionId)
                throw new InvalidOperationException("The product does not have this promotion!");

            return (product, promotion);
        }
    }
}
