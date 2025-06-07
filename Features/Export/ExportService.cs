namespace NutriBest.Server.Features.Export
{
    using System.Text;
    using System.Collections.Generic;
    using NutriBest.Server.Utilities;
    using NutriBest.Server.Features.Brands.Models;
    using NutriBest.Server.Features.Orders.Models;
    using NutriBest.Server.Features.Reports.Models;
    using NutriBest.Server.Features.Profile.Models;
    using NutriBest.Server.Features.Products.Models;
    using NutriBest.Server.Features.Packages.Models;
    using NutriBest.Server.Features.Flavours.Models;
    using NutriBest.Server.Features.PromoCodes.Models;
    using NutriBest.Server.Features.Promotions.Models;
    using NutriBest.Server.Features.Categories.Models;
    using NutriBest.Server.Features.Newsletter.Models;
    using NutriBest.Server.Features.ShippingDiscounts.Models;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;

    public class ExportService : IExportService, ITransientService
    {
        public string BrandsCsv(IEnumerable<BrandServiceModel> brands)
        {
            var csv = new StringBuilder();
            csv.AppendLine("BrandName,Description");

            foreach (var brand in brands)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(brand.Name ?? "")},{CsvHelper.EscapeCsvValue(brand.Description ?? "-")}");
            }

            return csv.ToString();
        }

        public string CategoriesCsv(IEnumerable<CategoryServiceModel> categories)
        {
            var csv = new StringBuilder();
            csv.AppendLine("CategoryName");

            foreach (var category in categories)
            {
                csv.AppendLine(CsvHelper.EscapeCsvValue(category.Name ?? ""));
            }

            return csv.ToString();
        }

        public string FlavoursCsv(IEnumerable<FlavourServiceModel> flavours)
        {
            var csv = new StringBuilder();
            csv.AppendLine("FlavourName");

            foreach (var flavour in flavours)
            {
                csv.AppendLine(CsvHelper.EscapeCsvValue(flavour.Name));
            }

            return csv.ToString();
        }

        public string NewsletterCsv(IEnumerable<SubscriberServiceModel> subscribers)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Email,Name,RegisteredOn,PhoneNumber,IsAnonymous,HasOrders,TotalOrders");

            foreach (var subscriber in subscribers)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(subscriber.Email)},{CsvHelper.EscapeCsvValue(subscriber.Name ?? "-")},{CsvHelper.EscapeCsvValue(subscriber.RegisteredOn.ToString())},{CsvHelper.EscapeCsvValue(subscriber.PhoneNumber ?? "")},{CsvHelper.EscapeCsvValue(subscriber.IsAnonymous.ToString())},{CsvHelper.EscapeCsvValue(subscriber.HasOrders.ToString())},{CsvHelper.EscapeCsvValue(subscriber.TotalOrders.ToString())}");
            }

            return csv.ToString();
        }

        public string OrdersCsv(IEnumerable<OrderListingServiceModel> orders)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Id,IsFinished,IsConfirmed,MadeOn,CustomerName,City,Country,Email,PhoneNumber,IsPaid,IsShipped,PaymentMethod,IsAnonymous,TotalPrice");

            if (orders == null)
            {
                return csv.ToString();
            }

            foreach (var order in orders)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(order.OrderId.ToString())},{CsvHelper.EscapeCsvValue(order.IsFinished.ToString())},{CsvHelper.EscapeCsvValue(order.IsConfirmed.ToString())},{CsvHelper.EscapeCsvValue(order.MadeOn.ToString())},{CsvHelper.EscapeCsvValue(order.CustomerName)},{CsvHelper.EscapeCsvValue(order.City)},{CsvHelper.EscapeCsvValue(order.Country)},{CsvHelper.EscapeCsvValue(order.Email)},{CsvHelper.EscapeCsvValue(order.PhoneNumber ?? "-")},{CsvHelper.EscapeCsvValue(order.IsPaid.ToString())},{CsvHelper.EscapeCsvValue(order.IsShipped.ToString())},{CsvHelper.EscapeCsvValue(order.PaymentMethod)},{CsvHelper.EscapeCsvValue(order.IsAnonymous.ToString())},{CsvHelper.EscapeCsvValue(order.TotalPrice.ToString())}");
            }

            return csv.ToString();
        }

        public string OrdersSummaryCsv(OrdersSummaryServiceModel summary)
        {
            var csv = new StringBuilder();
            csv.AppendLine("TotalProducts,TotalOrders,TotalDiscounts,TotalPriceWithoutDiscount,TotalPrice");

            if (summary == null)
            {
                return csv.ToString();
            }
            csv.AppendLine($"{CsvHelper.EscapeCsvValue($"{summary.TotalProducts}")},{CsvHelper.EscapeCsvValue($"{summary.TotalOrders}")},{CsvHelper.EscapeCsvValue($"{summary.TotalDiscounts} BGN")},{CsvHelper.EscapeCsvValue($"{summary.TotalPriceWithoutDiscount} BGN")},{CsvHelper.EscapeCsvValue($"{summary.TotalPrice} BGN")}");

            return csv.ToString();
        }

        public string PackagesCsv(IEnumerable<PackageServiceModel> packages)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Grams");

            foreach (var package in packages)
            {
                csv.AppendLine(CsvHelper.EscapeCsvValue(package.Grams.ToString()));
            }

            return csv.ToString();
        }

        public string ProductsCsv(IEnumerable<ProductListingServiceModel> products)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Id,Name,StartingPrice,Brand,Description,Categories,PromotionId");

            if (products == null)
            {
                return csv.ToString();
            }

            foreach (var product in products)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(product.ProductId.ToString())},{CsvHelper.EscapeCsvValue(product.Name)},{CsvHelper.EscapeCsvValue($"{product.Price} BGN")},{CsvHelper.EscapeCsvValue(product.Brand)},{CsvHelper.EscapeCsvValue(product.Description)},{CsvHelper.EscapeCsvValue(string.Join(";", product.Categories))},{CsvHelper.EscapeCsvValue(product.PromotionId?.ToString() ?? "-")}");
            }

            return csv.ToString();
        }

        public string ProfilesCsv(IEnumerable<ProfileListingServiceModel> profiles)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Id,Name,Email,MadeOn,IsDeleted,PhoneNumber,City,TotalOrders,TotalSpent");

            foreach (var profile in profiles)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(profile.ProfileId)},{CsvHelper.EscapeCsvValue(profile.Name ?? "-")},{CsvHelper.EscapeCsvValue(profile.Email ?? "-")},{CsvHelper.EscapeCsvValue(profile.MadeOn.ToString())},{CsvHelper.EscapeCsvValue(profile.IsDeleted.ToString())},{CsvHelper.EscapeCsvValue(profile.PhoneNumber ?? "-")},{CsvHelper.EscapeCsvValue(profile.City ?? "-")},{CsvHelper.EscapeCsvValue(profile.TotalOrders.ToString())},{CsvHelper.EscapeCsvValue(profile.TotalSpent.ToString())}");
            }

            return csv.ToString();
        }

        public string PromoCodesCsv(IEnumerable<PromoCodeByDescriptionServiceModel> promoCodes)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Description,ExpiresIn,Codes");

            foreach (var promoCode in promoCodes)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(promoCode.Description)},{CsvHelper.EscapeCsvValue(promoCode.ExpireIn.ToString())},{CsvHelper.EscapeCsvValue(string.Join("; ", promoCode.PromoCodes ?? new List<string>()))}");
            }

            return csv.ToString();
        }

        public string PromotionsCsv(IEnumerable<PromotionServiceModel> promotions)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Id,Description,Brand,Category,DiscountAmount,DiscountPercentage,IsActive,StartDate,EndDate");

            foreach (var promotion in promotions)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(promotion.PromotionId.ToString() ?? "-")},{CsvHelper.EscapeCsvValue(promotion.Description ?? "-")},{CsvHelper.EscapeCsvValue(promotion.Brand ?? "-")},{CsvHelper.EscapeCsvValue(promotion.Category ?? "-")},{CsvHelper.EscapeCsvValue(promotion.DiscountAmount != null ? $"{promotion.DiscountAmount} BGN" : "-")},{CsvHelper.EscapeCsvValue(promotion.DiscountPercentage != null ? $"{promotion.DiscountPercentage}%" : "-")},{CsvHelper.EscapeCsvValue(promotion.IsActive.ToString())},{CsvHelper.EscapeCsvValue(promotion.StartDate.ToString())},{CsvHelper.EscapeCsvValue(promotion.EndDate != null ? $"{promotion.EndDate}" : "")}");
            }

            return csv.ToString();
        }

        public string PerformanceInfoCsv(PerformanceInfo report)
        {
            var csv = new StringBuilder();

            csv.AppendLine(CsvHelper.EscapeCsvValue("Top Selling Products"));
            csv.AppendLine("Id,Name,StartingPrice,PromotionId,SoldCount");

            foreach (var data in report.TopSellingProducts)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(data.Product.ProductId.ToString())},{CsvHelper.EscapeCsvValue(data.Product.Name)},{CsvHelper.EscapeCsvValue($"{data.Product.Price} BGN")},{CsvHelper.EscapeCsvValue(data.Product.PromotionId?.ToString() ?? "-")},{data.SoldCount}");
            }

            csv.AppendLine();

            csv.AppendLine(CsvHelper.EscapeCsvValue("Weak Products"));
            csv.AppendLine("Id,Name,StartingPrice,PromotionId,SoldCount");

            foreach (var data in report.WeakProducts)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(data.Product.ProductId.ToString())},{CsvHelper.EscapeCsvValue(data.Product.Name)},{CsvHelper.EscapeCsvValue($"{data.Product.Price} BGN")},{CsvHelper.EscapeCsvValue(data.Product.PromotionId?.ToString() ?? "-")},{data.SoldCount}");
            }

            csv.AppendLine();

            csv.AppendLine(CsvHelper.EscapeCsvValue("Top Categories"));
            csv.AppendLine("Category,SoldCount");

            foreach (var data in report.TopSellingCategories)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(data.CategoryName)},{CsvHelper.EscapeCsvValue(data.SoldCount.ToString())}");
            }

            csv.AppendLine();

            csv.AppendLine(CsvHelper.EscapeCsvValue("Weak Categories"));
            csv.AppendLine("Category,SoldCount");

            foreach (var data in report.WeakCategories)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(data.CategoryName)},{CsvHelper.EscapeCsvValue(data.SoldCount.ToString())}");
            }

            csv.AppendLine();

            csv.AppendLine(CsvHelper.EscapeCsvValue("Top Flavours"));
            csv.AppendLine("Category,SoldCount");

            foreach (var data in report.TopSellingFlavours)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(data.FlavourName)},{CsvHelper.EscapeCsvValue(data.SoldCount.ToString())}");
            }

            csv.AppendLine();

            csv.AppendLine(CsvHelper.EscapeCsvValue("Weak Categories"));
            csv.AppendLine("Category,SoldCount");

            foreach (var data in report.WeakFlavours)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(data.FlavourName)},{CsvHelper.EscapeCsvValue(data.SoldCount.ToString())}");
            }

            csv.AppendLine();

            csv.AppendLine(CsvHelper.EscapeCsvValue("Top Brands"));
            csv.AppendLine("Category,SoldCount");

            foreach (var data in report.TopSellingBrands)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(data.BrandName)},{CsvHelper.EscapeCsvValue(data.SoldCount.ToString())}");
            }

            csv.AppendLine();

            csv.AppendLine(CsvHelper.EscapeCsvValue("Weak Categories"));
            csv.AppendLine("Category,SoldCount");

            foreach (var data in report.WeakBrands)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(data.BrandName)},{CsvHelper.EscapeCsvValue(data.SoldCount.ToString())}");
            }

            return csv.ToString();
        }

        public string DemographicsInfoCsv(IEnumerable<SellingCityServiceModel> data)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Country,City,SoldCount");

            foreach (var entity in data)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(entity.Country)},{CsvHelper.EscapeCsvValue(entity.City)},{CsvHelper.EscapeCsvValue(entity.SoldCount.ToString())}");
            }

            return csv.ToString();
        }

        public string ShippingDiscountsCsv(IEnumerable<ShippingDiscountServiceModel> discounts)
        {
            var csv = new StringBuilder();
            csv.AppendLine("DiscountPercentage,Country,Description,MinimumPrice,EndDate");

            foreach (var shippingDiscount in discounts)
            {
                csv.AppendLine($"{CsvHelper.EscapeCsvValue(shippingDiscount.DiscountPercentage.ToString())},{CsvHelper.EscapeCsvValue(shippingDiscount.CountryName)},{CsvHelper.EscapeCsvValue(shippingDiscount.Description)},{CsvHelper.EscapeCsvValue(shippingDiscount.MinimumPrice != null ? $"{shippingDiscount.MinimumPrice} BGN" : "-")},{CsvHelper.EscapeCsvValue(shippingDiscount.EndDate.ToString() ?? "-")}");
            }

            return csv.ToString();
        }
    }
}
