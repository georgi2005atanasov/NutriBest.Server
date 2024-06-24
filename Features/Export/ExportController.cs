﻿namespace NutriBest.Server.Features.Export
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.Admin;
    using NutriBest.Server.Features.Brands;
    using NutriBest.Server.Features.Categories;
    using NutriBest.Server.Features.Flavours;
    using NutriBest.Server.Features.Newsletter;
    using NutriBest.Server.Features.Orders;
    using NutriBest.Server.Features.Packages;
    using NutriBest.Server.Features.Products;
    using NutriBest.Server.Features.PromoCodes;
    using NutriBest.Server.Features.Promotions;
    using NutriBest.Server.Features.Reports;
    using NutriBest.Server.Features.ShippingDiscounts;
    using NutriBest.Server.Utilities;
    using System.Text;

    public class ExportController
    {
        private readonly IExportService exportService;
        private readonly IBrandService brandService;
        private readonly ICategoryService categoryService;
        private readonly IFlavourService flavourService;
        private readonly INewsletterService newsletterService;
        private readonly IOrderService orderService;
        private readonly IPackageService packageService;
        private readonly IProductService productService;
        private readonly IProfileService profileService;
        private readonly IPromoCodeService promoCodeService;
        private readonly IPromotionService promotionService;
        private readonly IReportService reportService;
        private readonly IShippingDiscountService shippingDiscountService;

        public ExportController(IBrandService brandService,
            IExportService exportService,
            ICategoryService categoryService,
            IFlavourService flavourService,
            INewsletterService newsletterService,
            IOrderService orderService,
            IPackageService packageService,
            IProductService productService,
            IProfileService profileService,
            IPromoCodeService promoCodeService,
            IPromotionService promotionService,
            IReportService reportService,
            IShippingDiscountService shippingDiscountService)
        {  
            this.brandService = brandService;
            this.exportService = exportService;
            this.categoryService = categoryService;
            this.flavourService = flavourService;
            this.newsletterService = newsletterService;
            this.orderService = orderService;
            this.packageService = packageService;
            this.productService = productService;
            this.profileService = profileService;
            this.promoCodeService = promoCodeService;
            this.promotionService = promotionService;
            this.reportService = reportService;
            this.shippingDiscountService = shippingDiscountService;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/Brands/CSV")]
        public async Task<FileContentResult?> GetCsvBrands()
        {
            try
            {
                var brands = await brandService.All();
                var csv = exportService.BrandsCsv(brands);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "brands.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/Categories/CSV")]
        public async Task<FileContentResult?> GetCsvCategories()
        {
            try
            {
                var categories = await categoryService.All();
                var csv = exportService.CategoriesCsv(categories);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "categories.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/Flavours/CSV")]
        public async Task<FileContentResult?> GetCsvFlavours()
        {
            try
            {
                var flavours = await flavourService.All();
                var csv = exportService.FlavoursCsv(flavours);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "flavours.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/Newsletter/CSV")]
        public async Task<FileContentResult?> GetCsvNewsletter([FromQuery] string? search,
            [FromQuery] string? groupType)
        {
            try
            {
                var subscribers = await newsletterService.AllExportSubscribers(search, groupType);
                var csv = exportService.NewsletterCsv(subscribers.AsEnumerable());
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "newsletter.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/Orders/CSV")]
        public async Task<FileContentResult?> GetCsvOrders([FromQuery] string? search,
             [FromQuery] string? filters = "",
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null)
        {
            try
            {
                var (parsedStartDate, parsedEndDate) = DateTimeHelper.ParseDates(startDate, endDate);

                var orders = await orderService.AllForExport(search, filters, parsedStartDate, parsedEndDate);
                var csv = exportService.OrdersCsv(orders);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "orders.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/Orders/CSV/Summary")]
        public async Task<FileContentResult?> GetCsvOrdersSummary()
        {
            try
            {
                var summary = await orderService.Summary();
                var csv = exportService.OrdersSummaryCsv(summary);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "ordersSummary.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/Packages/CSV")]
        public async Task<FileContentResult?> GetCsvPackages()
        {
            try
            {
                var packages = await packageService.All();
                var csv = exportService.PackagesCsv(packages);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "packages.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/Products/CSV")]
        public async Task<FileContentResult?> GetCsvProducts([FromQuery] string? categories = "",
           [FromQuery] string? brand = "",
           [FromQuery] string? price = "",
           [FromQuery] string? alpha = "",
           [FromQuery] string? search = "",
           [FromQuery] string? priceRange = "",
           [FromQuery] string? quantities = "",
           [FromQuery] string? flavours = "")
        {
            try
            {
                var products = await productService.AllForExport(categories, brand, price, alpha, search, priceRange, quantities, flavours);
                var csv = exportService.ProductsCsv(products);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "products.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/Profiles/CSV")]
        public async Task<FileContentResult?> GetCsvProfiles([FromQuery] string? search,
            [FromQuery] string? groupType)
        {
            try
            {
                var users = await profileService.All(1, search, groupType); // Assuming this fetches your data
                var csv = exportService.ProfilesCsv(users.Profiles);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "profiles.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/PromoCodes/CSV")] // changed!!!
        public async Task<FileContentResult?> GetCsvPromoCodes()
        {
            try
            {
                var promoCodes = await promoCodeService.All();
                var csv = exportService.PromoCodesCsv(promoCodes);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "promoCodes.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/Promotions/CSV")]
        public async Task<FileContentResult?> GetCsvPromotions()
        {
            try
            {
                var promotions = await promotionService.All();
                var csv = exportService.PromotionsCsv(promotions);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "products.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/Reports/CSV/PerformanceInfo")]
        public async Task<FileContentResult?> GetCsvPerformanceInfo([FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null)
        {
            try
            {
                var (parsedStartDate, parsedEndDate) = DateTimeHelper.ParseDates(startDate, endDate);

                var products = await reportService.GetPerformanceInfo(parsedStartDate, parsedEndDate);
                var csv = exportService.PerformanceInfoCsv(products);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "performanceInfo.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/Reports/CSV/DemographicsInfo")]
        public async Task<FileContentResult?> GetCsvDemographicsInfo([FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null)
        {
            try
            {
                var (parsedStartDate, parsedEndDate) = DateTimeHelper.ParseDates(startDate, endDate);

                var topCities = await reportService.GetTopCities(parsedStartDate, parsedEndDate);
                var csv = exportService.DemographicsInfoCsv(topCities);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "demographics.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }


        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("/ShippingDiscounts/CSV")] //changed!!!
        public async Task<FileContentResult?> GetCsv()
        {
            try
            {
                var shippingDiscounts = await shippingDiscountService.All();
                var csv = exportService.ShippingDiscountsCsv(shippingDiscounts.ShippingDiscounts);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "categories.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}