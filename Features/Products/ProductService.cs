namespace NutriBest.Server.Features.Products
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
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
            string? brand,
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
            query = this.GetByBrand(query, brand ?? "");

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
                             Brand = x.Brand.Name
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
            string brandName,
            decimal price,
            List<int> categoriesIds,
            List<ProductSpecsServiceModel> productSpecs,
            string imageData,
            string contentType)
        {
            var brand = await db.Brands
                    .FirstOrDefaultAsync(x => x.Name == brandName);

            if (brand == null)
                throw new ArgumentNullException("Invalid Brand!");

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
                Brand = brand,
                CreatedOn = DateTime.Now,
                Quantity = 0
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

            await db.SaveChangesAsync(); // must be aware it is finished before creating the
                                         // productPackageFlavour entities

            await CreateProductSpecs(product, productSpecs);

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
            string brandName,
            decimal price,
            List<int> categoriesIds,
            List<ProductSpecsServiceModel> productSpecs,
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
            product.ProductPackageFlavours = new List<ProductPackageFlavour>();
            product.Quantity = 0;

            var brand = await db.Brands
                    .FirstOrDefaultAsync(x => x.Name == brandName);

            if (brand == null)
                throw new ArgumentNullException("Invalid Brand!");

            product.Brand = brand;

            //delete and then recreate product categories

            var existingCategories = db.ProductsCategories
                .Where(pc => pc.ProductId == productId);

            db.ProductsCategories.RemoveRange(existingCategories);

            foreach (var id in categoriesIds)
            {
                product.ProductsCategories
                    .Add(new ProductCategory { CategoryId = id });
            }

            //

            //delete and then recreate product package flavour table rows

            var existingProductSpecs = db.ProductsPackagesFlavours
                .Where(pc => pc.ProductId == productId);

            db.ProductsPackagesFlavours.RemoveRange(existingProductSpecs);

            //

            await db.SaveChangesAsync();

            await CreateProductSpecs(product, productSpecs);

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

                await db.ProductsPackagesFlavours
                    .Where(x => x.ProductId == productId)
                    .ForEachAsync(ppf =>
                    {
                        if (ppf.ProductId == productId)
                            ppf.IsDeleted = true;
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

            var brand = await db.Brands
                .FirstOrDefaultAsync(x => x.Name == productListingModel.Brand);

            if (brand != null) //be aware, i added this 'if' without checking
                productWithPromotion.Brand = brand.Name;

            return productWithPromotion;
        }

        private async Task CreateProductSpecs(Product product, List<ProductSpecsServiceModel> productSpecs)
        {
            foreach (var productSpec in productSpecs)
            {
                var package = await db.Packages
                    .FirstOrDefaultAsync(x => x.Grams == productSpec.Grams); // must be initially seeded

                if (package == null)
                    throw new ArgumentNullException("Invalid package!");

                var flavour = await db.Flavours
                    .FirstOrDefaultAsync(x => x.FlavourName == productSpec.Flavour); // must be initially seeded

                if (flavour == null)
                    throw new ArgumentNullException("Invalid flavour!");

                product.Quantity += productSpec.Quantity;

                var productPackageFlavour = new ProductPackageFlavour
                {
                    ProductId = product.ProductId,
                    PackageId = package.Id,
                    FlavourId = flavour.Id,
                    Quantity = productSpec.Quantity
                };

                db.ProductsPackagesFlavours.Add(productPackageFlavour);
            }
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
