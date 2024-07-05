using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Products
{
    using System.Globalization;
    using Microsoft.EntityFrameworkCore;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Promotions;
    using NutriBest.Server.Features.Products.Models;
    using NutriBest.Server.Features.Products.Factories;
    using NutriBest.Server.Features.Products.Extensions;
    using NutriBest.Server.Infrastructure.Services;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;
    using static ServicesConstants.PaginationConstants; // make separate constants class
    using static ErrorMessages.BrandsController;
    using static ErrorMessages.ProductsController;
    using static ErrorMessages.PackagesController;
    using static ErrorMessages.FlavoursController;
    using static ErrorMessages.PromotionsController;

    public class ProductService : IProductService, ITransientService
    {
        private readonly NutriBestDbContext db;
        private readonly IPromotionService promotionService;
        private readonly ICurrentUserService currentUserService;
        private readonly IMapper mapper;

        public ProductService(NutriBestDbContext db,
            IPromotionService promotionService,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            this.db = db;
            this.promotionService = promotionService;
            this.currentUserService = currentUserService;
            this.mapper = mapper;
        }

        public async Task<AllProductsServiceModel> All(int page,
            string? categoriesFilter,
            string? brand,
            string? priceFilter,
            string? alphaFilter,
            string? productsView,
            string? search,
            string? priceRange,
            string? quantities,
            string? flavours)
        {
            var query = db.Products.AsQueryable();

            var maxPrice = (int)Math.Ceiling(query.Select(x => x.MaximumPrice).Max());

            query = this.SelectByCategories(query, categoriesFilter ?? "");
            query = this.GetBySearch(query, search ?? "");
            query = this.GetByPriceRangeWithPromotions(query, priceRange ?? "");
            query = this.GetByBrand(query, brand ?? "");
            query = this.GetByQuantity(query, quantities ?? "");
            query = this.GetByFlavours(query, flavours ?? "");

            var productsCount = query.Count();

            int pagesToSkip = (page - 1) * ((productsView == "all") ? ProductsPerPage : ProductsPerTable);

            var queryProducts = query
                .OrderByDescending(x => x.CreatedOn)
                         .Select(x => new ProductListingServiceModel
                         {
                             ProductId = x.ProductId,
                             Name = x.Name,
                             Price = x.StartingPrice,
                             Categories = x.ProductsCategories
                             .Select(c => c.Category.Name)
                             .ToList(),
                             Quantity = x.Quantity,
                             PromotionId = x.PromotionId,
                             Description = x.Description,
                             Brand = x.Brand!.Name // be aware
                         })
                         .AsQueryable();

            queryProducts = this.OrderByName(queryProducts, alphaFilter ?? "");
            queryProducts = this.OrderByPrice(queryProducts, priceFilter ?? "");
            queryProducts = queryProducts
                .Skip(pagesToSkip)
                .Take((productsView == "all") ? ProductsPerPage : ProductsPerTable);

            var products = await Task.Run(() => queryProducts.ToList());

            await GetPromotionPercentage(products);

            var productsRows = this.GetProductsRows(products,
                (productsView == "all") ? ProductsPerRow : ProductsPerRowInTable);

            return new AllProductsServiceModel
            {
                ProductsRows = productsRows,
                Count = productsCount,
                MaxPrice = maxPrice
            };
        }

        public async Task<List<ProductListingServiceModel>> AllForExport(string? categories,
            string? brand,
            string? priceFilter,
            string? alphaFilter,
            string? search,
            string? priceRange,
            string? quantities,
            string? flavours)
        {
            var allProducts = new List<ProductListingServiceModel>();

            int currentPage = 1;
            while (true)
            {
                var products = await All(currentPage, categories, brand, priceFilter, alphaFilter, null, search, priceRange, quantities, flavours);

                if (products.ProductsRows == null || products.ProductsRows.Count() == 0)
                    return allProducts;

                var productsToAdd = products.ProductsRows.SelectMany(x => x);

                foreach (var product in productsToAdd)
                {
                    allProducts.Add(product);
                }

                currentPage++;
            }
        }

        public async Task<int> Create(string name,
            string description,
            string brandName,
            List<int> categoriesIds,
            List<ProductSpecsServiceModel> productSpecs,
            string imageData,
            string contentType)
        {
            var brand = await db.Brands
                    .FirstOrDefaultAsync(x => x.Name == brandName);

            if (brand == null)
                throw new ArgumentNullException(InvalidBrandName);

            var productImage = new ProductImage
            {
                ImageData = imageData,
                ContentType = contentType
            };

            var minPrice = decimal.MaxValue;

            var maxPrice = decimal.MinValue;

            foreach (var spec in productSpecs)
            {
                if (!decimal.TryParse(spec.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                    throw new FormatException($"Invalid price format: {spec.Price}");

                if (minPrice > price)
                    minPrice = price;

                if (maxPrice < price)
                    maxPrice = price;
            }

            var product = new Product
            {
                Name = name,
                Description = description,
                StartingPrice = minPrice,
                MaximumPrice = maxPrice,
                ProductImage = productImage,
                Brand = brand,
                CreatedOn = DateTime.UtcNow,
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

            await db.SaveChangesAsync();

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
                throw new ArgumentNullException(InvalidProduct);

            return product;
        }

        public async Task<List<ProductSpecsServiceModel>> GetSpecs(int id, string name)
        {
            var product = await db.Products
                .FirstOrDefaultAsync(x => x.ProductId == id && x.Name == name);

            if (product == null)
                throw new ArgumentNullException(InvalidProduct);

            var productPackageFlavours = await db.ProductsPackagesFlavours
                .Where(x => x.ProductId == id && x.Quantity > 0)
                .ToListAsync();

            var specs = new List<ProductSpecsServiceModel>();

            foreach (var productPackageFlavour in productPackageFlavours)
            {
                var spec = await ProductFactory
                    .CreateProductSpecsServiceModelAsync(db, productPackageFlavour);
                specs.Add(spec);
            }

            specs = specs
                .OrderBy(x => x.Grams)
                .ToList();

            return specs;
        }

        public async Task<int> Update(int productId,
            string name,
            string description,
            string brandName,
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
            product.StartingPrice = decimal.Parse(productSpecs.OrderBy(x => decimal.Parse(x.Price)).First().Price);
            product.MaximumPrice = decimal.Parse(productSpecs.OrderByDescending(x => decimal.Parse(x.Price)).First().Price);
            product.ProductImage = productImage;
            product.ProductsCategories = new List<ProductCategory>();
            product.ProductPackageFlavours = new List<ProductPackageFlavour>();
            product.Quantity = 0;

            var brand = await db.Brands
                    .FirstOrDefaultAsync(x => x.Name == brandName);

            if (brand == null)
                throw new ArgumentNullException(InvalidBrandName);

            product.Brand = brand;

            //delete and then recreate product categories

            var existingCategories = db.ProductsCategories
                .Where(pc => pc.ProductId == productId);

            db.ProductsCategories.RemoveRange(existingCategories);

            foreach (var id in categoriesIds)
            {
                product.ProductsCategories
                    .Add(new ProductCategory
                    {
                        //Category = db.Categories.Find(id)!,
                        CategoryId = id
                    });
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

                product.IsDeleted = true;
                product.DeletedBy = currentUserService.GetUserName();
                product.DeletedOn = DateTime.UtcNow;

                await db.ProductsPackagesFlavours
                    .Where(x => x.ProductId == productId)
                    .ForEachAsync(ppf =>
                    {
                        if (ppf.ProductId == productId)
                            ppf.IsDeleted = true;
                    });

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

                productImage.IsDeleted = true;
                productImage.DeletedBy = currentUserService.GetUserName();
                productImage.DeletedOn = DateTime.UtcNow;

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
                newPrice = product.StartingPrice * ((100 - (decimal)promotion.DiscountPercentage) / 100);

            if (promotion.DiscountAmount != null)
                newPrice = product.StartingPrice - (decimal)promotion.DiscountAmount;

            var productListingModel = await Get(productId);

            var productWithPromotion = new ProductWithPromotionDetailsServiceModel
            {
                Categories = productListingModel.Categories,
                Image = productListingModel.Image,
                Description = product.Description,
                Name = product.Name,
                Price = product.StartingPrice,
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
                    throw new ArgumentNullException(InvalidPackage);

                var flavour = await db.Flavours
                    .FirstOrDefaultAsync(x => x.FlavourName == productSpec.Flavour); // must be initially seeded

                if (flavour == null)
                    throw new ArgumentNullException(InvalidFlavour);

                product.Quantity += productSpec.Quantity;

                string currentPrice = productSpec.Price.Replace(',', '.');
                decimal.TryParse(currentPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price);

                var productPackageFlavour = new ProductPackageFlavour
                {
                    ProductId = product.ProductId,
                    PackageId = package.Id,
                    FlavourId = flavour.Id,
                    Quantity = productSpec.Quantity,
                    Price = price // be aware
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

            if (product == null)
                throw new ArgumentNullException(InvalidProduct);

            if (promotion == null)
                throw new ArgumentNullException(InvalidPromotion);

            if (product.PromotionId != promotionId)
                throw new InvalidOperationException(ProductDoesNotHaveThisPromotion);

            return (product, promotion);
        }

        public async Task<int> PartialUpdate(int id, string? description)
        {
            var product = await db.Products
                .FirstOrDefaultAsync(x => x.ProductId == id);

            if (product == null)
                throw new ArgumentNullException(InvalidProduct);

            if (description != null)
                product.Description = description;

            await db.SaveChangesAsync();

            return id;
        }

        public async Task<List<ProductListingServiceModel>> GetRelatedProducts(List<string>? categories, int productId)
        {
            var products = categories != null ? await db.Products
                .Where(x => x.ProductsCategories
                            .Any(x => categories.Contains(x.Category.Name)) && x.ProductId != productId)
                .Select(x => new ProductListingServiceModel
                {
                    ProductId = x.ProductId,
                    Name = x.Name,
                    Price = x.StartingPrice,
                    Categories = x.ProductsCategories
                             .Select(c => c.Category.Name)
                             .ToList(),
                    Quantity = x.Quantity,
                    PromotionId = x.PromotionId,
                    Brand = x.Brand!.Name // be aware
                })
                .Take(4)
                .ToListAsync() :
            new List<ProductListingServiceModel>();

            await GetPromotionPercentage(products);

            return products;
        }

        private async Task GetPromotionPercentage(List<ProductListingServiceModel> products)
        {
            foreach (var product in products)
            {
                if (product.PromotionId != null)
                {
                    try
                    {
                        var promotion = await promotionService.Get((int)product.PromotionId);

                        if (!promotion.IsActive)
                            throw new InvalidOperationException(PromotionIsNotActive);

                        if (promotion != null && promotion.DiscountPercentage != null)
                            product.DiscountPercentage = promotion.DiscountPercentage;

                        if (promotion != null && promotion.DiscountAmount != null)
                            product.DiscountPercentage = promotion.DiscountAmount * 100 / product.Price;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
        }

        public async Task<ProductPriceQuantityServiceModel?> GetCurrentPriceWithQuantity(int productId, string flavour, int grams)
        {
            var productPackageFlavour = await db.ProductsPackagesFlavours
                .Include(x => x.Flavour)
                .Include(x => x.Package)
                .SingleOrDefaultAsync(x => x.Flavour!.FlavourName == flavour &&
                x.ProductId == productId &&
                x.Package!.Grams == grams);

            if (productPackageFlavour == null)
                return null;

            var priceWithQuantityModel = new ProductPriceQuantityServiceModel
            {
                Price = productPackageFlavour.Price,
                Quantity = productPackageFlavour.Quantity
            };

            return priceWithQuantityModel;
        }
    }
}
