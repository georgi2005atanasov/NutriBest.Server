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
            => this.db = db;

        public async Task<PerformanceInfo> GetPerformanceInfo(DateTime? startDate, DateTime? endDate)
        {
            var performanceInfo = new PerformanceInfo
            {
                TopSellingProducts = await GetTopSellingProducts(startDate, endDate),
                TopSellingBrands = await GetTopSellingBrands(startDate, endDate),
                TopSellingCategories = await GetTopSellingCategories(startDate, endDate),
                TopSellingFlavours = await GetTopSellingFlavours(startDate, endDate),
                WeakProducts = await GetWeakProducts(startDate, endDate),
                WeakBrands = await GetWeakBrands(startDate, endDate),
                WeakCategories = await GetWeakCategories(startDate, endDate),
                WeakFlavours = await GetWeakFlavours(startDate, endDate),
                OverallSalesVolume = await GetOverallSalesVolume(startDate, endDate) ?? 0
            };

            return performanceInfo;
        }

        public async Task<decimal?> GetOverallSalesVolume(DateTime? startDate, DateTime? endDate)
            => await db.Orders
                .Where(x => startDate == null || x.CreatedOn >= startDate)
                .Where(x => endDate == null || x.CreatedOn <= endDate)
                .Select(x => x.Cart!.TotalProducts + x.Cart.ShippingPrice)
                .AverageAsync();

        public async Task<List<SellingProductServiceModel>> GetTopSellingProducts(DateTime? startDate, DateTime? endDate)
        {
            var products = await db.CartProducts
                .IgnoreQueryFilters()
                .Where(x => startDate == null || x.CreatedOn >= startDate)
                .Where(x => endDate == null || x.CreatedOn <= endDate)
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

        public async Task<List<SellingBrandServiceModel>> GetTopSellingBrands(DateTime? startDate, DateTime? endDate)
        {
            var brands = await db.CartProducts
                .IgnoreQueryFilters()
                .Where(x => startDate == null || x.CreatedOn >= startDate)
                .Where(x => endDate == null || x.CreatedOn <= endDate)
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

        public async Task<List<SellingFlavourServiceModel>> GetTopSellingFlavours(DateTime? startDate, DateTime? endDate)
        {
            var flavours = await db.CartProducts
                .IgnoreQueryFilters()
                .Where(x => startDate == null || x.CreatedOn >= startDate)
                .Where(x => endDate == null || x.CreatedOn <= endDate)
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

        public async Task<List<SellingCategoryServiceModel>> GetTopSellingCategories(DateTime? startDate, DateTime? endDate)
        {
            var categories = await db.Categories
                .IgnoreQueryFilters()
                .GroupBy(x => x.Id)
                .Select(y => new SellingCategoryServiceModel
                {
                    CategoryName = db.Categories.First(x => x.Id == y.Key).Name,
                    SoldCount = db.CartProducts
                    .Where(x => startDate == null || x.CreatedOn >= startDate)
                    .Where(x => endDate == null || x.CreatedOn <= endDate)
                    .Where(x => x.Product!.ProductsCategories.Any(x => x.CategoryId == y.Key))
                    .Select(x => x.Count)
                    .Sum()
                })
                .OrderByDescending(x => x.SoldCount)
                .Take(10)
                .ToListAsync();

            return categories;
        }

        public async Task<List<SellingProductServiceModel>> GetWeakProducts(DateTime? startDate, DateTime? endDate)
        {
            var products = await db.CartProducts
                .IgnoreQueryFilters()
                .Where(x => startDate == null || x.CreatedOn >= startDate)
                .Where(x => endDate == null || x.CreatedOn <= endDate)
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
                        PromotionId = db.Products.First(x => x.ProductId == y.Key).PromotionId,
                        Brand =  db.Products.First(x => x.ProductId == y.Key).Brand!.Name
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

        public async Task<List<SellingBrandServiceModel>> GetWeakBrands(DateTime? startDate, DateTime? endDate)
        {
            var brands = await db.CartProducts
                .IgnoreQueryFilters()
                .Where(x => startDate == null || x.CreatedOn >= startDate)
                .Where(x => endDate == null || x.CreatedOn <= endDate)
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

        public async Task<List<SellingFlavourServiceModel>> GetWeakFlavours(DateTime? startDate, DateTime? endDate)
        {
            var flavours = await db.CartProducts
                .IgnoreQueryFilters()
                .Where(x => startDate == null || x.CreatedOn >= startDate)
                .Where(x => endDate == null || x.CreatedOn <= endDate)
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

        public async Task<List<SellingCategoryServiceModel>> GetWeakCategories(DateTime? startDate, DateTime? endDate)
        {
            var categories = await db.Categories
                .IgnoreQueryFilters()
                .GroupBy(x => x.Id)
                .Select(y => new SellingCategoryServiceModel
                {
                    CategoryName = db.Categories.First(x => x.Id == y.Key).Name,
                    SoldCount = db.CartProducts
                    .Where(x => startDate == null || x.CreatedOn >= startDate)
                    .Where(x => endDate == null || x.CreatedOn <= endDate)
                    .Where(x => x.Product!.ProductsCategories.Any(x => x.CategoryId == y.Key))
                    .Select(x => x.Count)
                    .Sum()
                })
                .OrderBy(x => x.SoldCount)
                .Take(10)
                .ToListAsync();

            return categories;
        }

        public async Task<List<SellingCityServiceModel>> GetTopCities(DateTime? startDate, DateTime? endDate)
        {
            var query = from orderDetail in db.OrdersDetails
                        join order in db.Orders on orderDetail.Id equals order.Id
                        join address in db.Addresses on orderDetail.AddressId equals address.Id
                        where (startDate == null || order.CreatedOn >= startDate) &&
                              (endDate == null || order.CreatedOn <= endDate)
                        group orderDetail by address.CityId into g
                        select new
                        {
                            CityId = g.Key,
                            SoldCount = g.Count()
                        };

            var cities = await query
                .Select(x => new SellingCityServiceModel
                {
                    Country = db.Countries
                        .First(y => y.Cities.Any(z => z.Id == x.CityId))
                        .CountryName,
                    City = db.Cities
                        .First(y => y.Id == x.CityId)
                        .CityName,
                    SoldCount = x.SoldCount
                })
                .OrderByDescending(x => x.SoldCount)
                .Take(30)
                .ToListAsync();

            return cities;
        }
    }
}
