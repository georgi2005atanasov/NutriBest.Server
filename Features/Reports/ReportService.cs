namespace NutriBest.Server.Features.Reports
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Products.Models;
    using NutriBest.Server.Features.Reports.Models;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;

    public class ReportService : IReportService, ITransientService
    {
        private readonly NutriBestDbContext db;

        public ReportService(NutriBestDbContext db)
        {
            this.db = db;
        }

        public async Task<TopSellingProductsServiceModel> GetTopSellingProducts()
        {
            var products = await db.CartProducts
                .GroupBy(x => x.ProductId)
                .Select(y => new TopSellingProductServiceModel
                {
                    Product = new ProductListingServiceModel
                    {
                        ProductId = y.Key,
                        Name = db.Products.First(x => x.ProductId == y.Key).Name,
                        Price = db.Products.First(x => x.ProductId == y.Key).StartingPrice,
                        Categories = db.ProductsCategories
                             .Where(x => x.ProductId == y.Key)
                             .Select(c => c.Category.Name)
                             .ToList(),
                        Quantity = db.Products.First(x => x.ProductId == y.Key).Quantity,
                        PromotionId = db.Products.First(x => x.ProductId == y.Key).PromotionId
                    },
                    SoldCount = db.CartProducts
                    .Where(x => x.ProductId == y.Key)
                    .Select(x => x.Count)
                    .Sum()
                })
                .OrderByDescending(x => x.SoldCount)
                .Take(5)
                .ToListAsync();

            var topProducts = new TopSellingProductsServiceModel
            {
                Products = products
            };

            return topProducts;
        }

        public async Task<TopSellingBrandsServiceModel> GetTopSellingBrands()
        {
            var brands = await db.CartProducts
                .GroupBy(x => (int)db.Products.First(y => y.ProductId == x.ProductId).BrandId!)
                .Select(y => new TopSellingBrandServiceModel
                {
                    BrandName = db.Brands.First(x => x.Id == y.Key).Name,
                    SoldCount = db
                    .CartProducts
                    .Include(x => x.Product)
                    .Where(x => x.Product!.BrandId == y.Key)
                    .Select(x => x.Count)
                    .Sum()
                })
                .OrderByDescending(x => x.SoldCount)
                .Where(x => x.SoldCount > 0)
                .Take(5)
                .ToListAsync();

            var topBrands = new TopSellingBrandsServiceModel
            {
                Brands = brands
            };

            return topBrands;
        }

        public async Task<TopSellingFlavoursServiceModel> GetTopSellingFlavours()
        {
            var flavours = await db.CartProducts
                .GroupBy(x => x.FlavourId)
                .Select(y => new TopSellingFlavourServiceModel
                {
                    FlavourName = db.Flavours.First(x => x.Id == y.Key).FlavourName,
                    SoldCount = db.CartProducts
                    .Where(x => x.FlavourId == y.Key)
                    .Select(x => x.Count)
                    .Sum()
                })
                .OrderByDescending(x => x.SoldCount)
                .Take(5)
                .ToListAsync();

            var topFlavours = new TopSellingFlavoursServiceModel
            {
                Flavours = flavours
            };

            return topFlavours;
        }

        public async Task<TopSellingCategoriesServiceModel> GetTopSellingCategories()
        {
            var categories = await db.Categories
                .GroupBy(x => x.Id)
                .Select(y => new TopSellingCategoryServiceModel
                {
                    CategoryName = db.Categories.First(x => x.Id == y.Key).Name,
                    SoldCount = db.CartProducts
                    .Include(x => x.Product)
                    .Include(x => x.Product!.ProductsCategories)
                    .Where(x => x.Product!.ProductsCategories.Any(x => x.CategoryId == y.Key))
                    .Select(x => x.Count)
                    .Sum()
                })
                .OrderByDescending(x => x.SoldCount)
                .Take(5)
                .ToListAsync();

            var topCategories = new TopSellingCategoriesServiceModel
            {
                Categories = categories
            };

            return topCategories;
        }
    }
}
