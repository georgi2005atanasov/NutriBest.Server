namespace NutriBest.Server.Features.Products.Extensions
{
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Products.Models;
    using NutriBest.Server.Features.Promotions;

    public static class ProductServiceExtensions
    {
        public static List<List<ProductListingServiceModel>> GetProductsRows(this IProductService service, List<ProductListingServiceModel> products, int productsPerRow)
        {
            var productsRows = new List<List<ProductListingServiceModel>>();
            int i = 0;
            var row = new List<ProductListingServiceModel>();

            for (int j = i; j < products.Count; j++)
            {
                if (j % productsPerRow == 0 && j != 0)
                {
                    productsRows.Add(row);
                    row = new List<ProductListingServiceModel>();
                }

                row.Add(products[j]);
            }

            if (row.Count > 0)
                productsRows.Add(row);

            return productsRows;
        }

        public static bool HasValidCategory(this IProductService service, List<string> categories, string categoriesFilter)
        {
            var categoriesFilterList = categoriesFilter.Split();

            foreach (var filterCategory in categoriesFilterList)
            {
                foreach (var category in categories)
                {
                    if (filterCategory == category)
                        return true;
                }
            }

            return false;
        }

        public static IQueryable<Product> SelectByCategories(this IProductService service, IQueryable<Product> query, string categoriesFilter = "")
        {
            if (!string.IsNullOrEmpty(categoriesFilter))
            {
                var categoriesToCheck = categoriesFilter
                    .Split(" and ")
                    .ToList();

                query = query
                    .Where(x => x.ProductsCategories
                                .Select(x => x.Category.Name)
                                .Any(x => categoriesToCheck.Contains(x)));
            }

            return query;
        }

        public static IQueryable<ProductListingServiceModel> OrderByPrice(this IProductService service, IQueryable<ProductListingServiceModel> queryProducts, string priceFilter = "")
        {
            if (!string.IsNullOrEmpty(priceFilter))
            {
                if (priceFilter == "desc")
                    queryProducts = queryProducts
                        .OrderByDescending(x => x.Price);
                else if (priceFilter == "asc")
                    queryProducts = queryProducts
                        .OrderBy(x => x.Price);
            }

            return queryProducts;
        }

        public static IQueryable<ProductListingServiceModel> OrderByName(this IProductService service, IQueryable<ProductListingServiceModel> queryProducts, string alphaFilter = "")
        {
            if (!string.IsNullOrEmpty(alphaFilter))
            {
                if (alphaFilter == "desc")
                    queryProducts = queryProducts
                        .OrderByDescending(x => x.Name);
                else if (alphaFilter == "asc")
                    queryProducts = queryProducts
                        .OrderBy(x => x.Name);
            }

            return queryProducts;
        }

        public static IQueryable<Product> GetBySearch(this IProductService service, IQueryable<Product> queryProducts, string search)
        {
            if (search != "")
            {
                queryProducts = queryProducts
                    .Where(x => x.Name.ToLower().Contains(search.ToLower()));
            }

            return queryProducts;
        }

        public static IQueryable<Product> GetByQuantity(this IProductService service, IQueryable<Product> query, string quantities)
        {
            if (!string.IsNullOrEmpty(quantities))
            {
                var quantitiesToCheck = quantities
                    .Split()
                    .Select(int.Parse)
                    .ToList();

                query = query
                    .Where(x => x.ProductPackageFlavours
                                .Select(x => x.Package)
                                .Any(x => quantitiesToCheck.Contains(x!.Grams))); // be aware
            }

            return query;
        }

        public static IQueryable<Product> GetByFlavours(this IProductService service, IQueryable<Product> query, string flavours)
        {
            if (!string.IsNullOrEmpty(flavours))
            {
                var flavoursToCheck = flavours
                    .Split(" andAlso ")
                    .ToList();

                query = query
                    .Where(x => x.ProductPackageFlavours
                                .Select(x => x.Flavour)
                                .Any(x => flavoursToCheck.Contains(x!.FlavourName))); // be aware
            }

            return query;
        }

        public static IQueryable<Product> GetByBrand(this IProductService service, IQueryable<Product> queryProducts, string brand)
            => brand != "" ? queryProducts.Where(x => x.Brand!.Name == brand) : queryProducts; // be aware

        public static IQueryable<Product> GetByPriceRangeWithPromotions(this IProductService service, IQueryable<Product> query, string priceRange = "")
        {
            if (priceRange == "")
            {
                return query;
            }

            try
            {
                var numbers = priceRange.Split();
                var minPrice = int.Parse(numbers[0]);
                var maxPrice = int.Parse(numbers[1]);

                query = query
                    .Where(x => x.StartingPrice >= minPrice && x.StartingPrice <= maxPrice || x.PromotionId != null);

                return query;
            }
            catch (Exception)
            {
                return query;
            }
        }

        public static async Task<IQueryable<ProductListingServiceModel>> CleanPromotions(this IProductService service,
            IPromotionService promotionService,
            IQueryable<ProductListingServiceModel> query,
            string priceRange = "")
        {
            if (priceRange == "")
            {
                IQueryable<ProductListingServiceModel> productPromotions = query
                    .Where(x => x.PromotionId != null);

                return productPromotions;
            }

            try
            {
                var numbers = priceRange.Split();
                var minPrice = int.Parse(numbers[0]);
                var maxPrice = int.Parse(numbers[1]);

                IQueryable<ProductListingServiceModel> productPromotions = query
                    .Where(x => x.PromotionId != null);

                var invalidProductIds = new HashSet<int>();

                foreach (var x in productPromotions)
                {
                    var promotion = await promotionService.Get((int)x.PromotionId!); //be aware
                    decimal? priceToCheck = null;

                    if (promotion.DiscountPercentage != null)
                    {
                        priceToCheck = x.Price * ((100 - promotion.DiscountPercentage) / 100.0m);
                    }
                    else if (promotion.DiscountAmount != null)
                    {
                        priceToCheck = x.Price - promotion.DiscountAmount;
                    }
                    else
                    {
                        invalidProductIds.Add(x.ProductId);
                    }

                    if (priceToCheck != null && priceToCheck >= minPrice && priceToCheck <= maxPrice)
                    {
                        continue;
                    }
                    else
                    {
                        invalidProductIds.Add(x.ProductId);
                    }
                }

                productPromotions = productPromotions.Where(x => !invalidProductIds.Contains(x.ProductId));

                return productPromotions;
            }
            catch (Exception)
            {
                return query;
            }
        }
    }
}
