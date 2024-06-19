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

        public async Task<PerformanceInfo> GetPerformanceInfo()
        {
            var performanceInfo = new PerformanceInfo
            {
                TopSellingProducts = await GetTopSellingProducts(),
                TopSellingBrands = await GetTopSellingBrands(),
                TopSellingCategories = await GetTopSellingCategories(),
                TopSellingFlavours = await GetTopSellingFlavours(),
                WeakProducts = await GetWeakProducts(),
                WeakBrands = await GetWeakBrands(),
                WeakCategories = await GetWeakCategories(),
                WeakFlavours = await GetWeakFlavours(),
                OverallSalesVolume = await GetOverallSalesVolume() ?? 0
            };

            return performanceInfo;
        }

        public async Task<decimal?> GetOverallSalesVolume()
            => await db.Orders
                .Select(x => x.Cart!.TotalProducts + x.Cart.ShippingPrice)
                .AverageAsync();

        public async Task<List<SellingProductServiceModel>> GetTopSellingProducts()
        {
            var products = await db.CartProducts
                .IgnoreQueryFilters()
                .GroupBy(x => x.ProductId)
                .Select(y => new SellingProductServiceModel
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
                .Take(10)
                .ToListAsync();

            return products;
        }

        public async Task<List<SellingBrandServiceModel>> GetTopSellingBrands()
        {
            var brands = await db.CartProducts
                .IgnoreQueryFilters()
                .GroupBy(x => (int)db.Products.First(y => y.ProductId == x.ProductId).BrandId!)
                .Select(y => new SellingBrandServiceModel
                {
                    BrandName = db.Brands.First(x => x.Id == y.Key).Name,
                    SoldCount = db
                    .CartProducts
                    .Where(x => x.Product!.BrandId == y.Key)
                    .Select(x => x.Count)
                    .Sum()
                })
                .OrderByDescending(x => x.SoldCount)
                .Take(10)
                .ToListAsync();

            return brands;
        }

        public async Task<List<SellingFlavourServiceModel>> GetTopSellingFlavours()
        {
            var flavours = await db.CartProducts
                .IgnoreQueryFilters()
                .GroupBy(x => x.FlavourId)
                .Select(y => new SellingFlavourServiceModel
                {
                    FlavourName = db.Flavours.First(x => x.Id == y.Key).FlavourName,
                    SoldCount = db.CartProducts
                    .Where(x => x.FlavourId == y.Key)
                    .Select(x => x.Count)
                    .Sum()
                })
                .OrderByDescending(x => x.SoldCount)
                .Take(10)
                .ToListAsync();


            return flavours;
        }

        public async Task<List<SellingCategoryServiceModel>> GetTopSellingCategories()
        {
            var categories = await db.Categories
                .IgnoreQueryFilters()
                .GroupBy(x => x.Id)
                .Select(y => new SellingCategoryServiceModel
                {
                    CategoryName = db.Categories.First(x => x.Id == y.Key).Name,
                    SoldCount = db.CartProducts
                    .Where(x => x.Product!.ProductsCategories.Any(x => x.CategoryId == y.Key))
                    .Select(x => x.Count)
                    .Sum()
                })
                .OrderByDescending(x => x.SoldCount)
                .Take(10)
                .ToListAsync();

            return categories;
        }

        public async Task<List<SellingProductServiceModel>> GetWeakProducts()
        {
            var products = await db.CartProducts
                .IgnoreQueryFilters()
                .GroupBy(x => x.ProductId)
                .Select(y => new SellingProductServiceModel
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
                .OrderBy(x => x.SoldCount)
                .Take(10)
                .ToListAsync();

            return products;
        }

        public async Task<List<SellingBrandServiceModel>> GetWeakBrands()
        {
            var brands = await db.CartProducts
                .IgnoreQueryFilters()
                .GroupBy(x => (int)db.Products.First(y => y.ProductId == x.ProductId).BrandId!)
                .Select(y => new SellingBrandServiceModel
                {
                    BrandName = db.Brands.First(x => x.Id == y.Key).Name,
                    SoldCount = db
                    .CartProducts
                    .Where(x => x.Product!.BrandId == y.Key)
                    .Select(x => x.Count)
                    .Sum()
                })
                .OrderBy(x => x.SoldCount)
                .Take(10)
                .ToListAsync();

            return brands;
        }

        public async Task<List<SellingFlavourServiceModel>> GetWeakFlavours()
        {
            var flavours = await db.CartProducts
                .IgnoreQueryFilters()
               .GroupBy(x => x.FlavourId)
               .Select(y => new SellingFlavourServiceModel
               {
                   FlavourName = db.Flavours.First(x => x.Id == y.Key).FlavourName,
                   SoldCount = db.CartProducts
                   .Where(x => x.FlavourId == y.Key)
                   .Select(x => x.Count)
                   .Sum()
               })
               .OrderBy(x => x.SoldCount)
               .Take(10)
               .ToListAsync();


            return flavours;
        }

        public async Task<List<SellingCategoryServiceModel>> GetWeakCategories()
        {
            var categories = await db.Categories
                .IgnoreQueryFilters()
                .GroupBy(x => x.Id)
                .Select(y => new SellingCategoryServiceModel
                {
                    CategoryName = db.Categories.First(x => x.Id == y.Key).Name,
                    SoldCount = db.CartProducts
                    .Where(x => x.Product!.ProductsCategories.Any(x => x.CategoryId == y.Key))
                    .Select(x => x.Count)
                    .Sum()
                })
                .OrderBy(x => x.SoldCount)
                .Take(10)
                .ToListAsync();

            return categories;
        }

        public async Task<List<SellingCityServiceModel>> GetTopCities()
        {
            var cities = await db.OrdersDetails
                .Include(x => x.Address)
                .GroupBy(x => x.Address!.CityId)
                .Select(x => new SellingCityServiceModel
                {
                    Country = db.Countries
                                .First(y => y.Cities.Any(z => z.Id == x.Key))
                                .CountryName,
                    City = db.Cities
                                .First(y => y.Id == x.Key)
                                .CityName,
                    SoldCount = db.OrdersDetails
                                .Where(y => y.Address!.CityId == x.Key)
                                .Count()
                })
                .OrderByDescending(x => x.SoldCount)
                .Take(10)
                .ToListAsync();

            return cities;
        }
    }
}
