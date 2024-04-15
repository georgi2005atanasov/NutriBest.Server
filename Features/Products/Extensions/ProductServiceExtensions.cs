﻿using NutriBest.Server.Features.Products.Models;

namespace NutriBest.Server.Features.Products.Extensions
{
    using static ServicesConstants.PaginationConstants;
    public static class ProductServiceExtensions
    {
        public static List<List<ProductListingModel>> GetProductsRows(this IProductService service, List<ProductListingModel> products)
        {
            var productsRows = new List<List<ProductListingModel>>();
            int i = 0;
            var row = new List<ProductListingModel>();

            for (int j = i; j < products.Count; j++)
            {
                if (j % productsPerRow == 0 && j != 0)
                {
                    productsRows.Add(row);
                    row = new List<ProductListingModel>();
                }

                row.Add(products[j]);
            }

            if (row.Count > 0)
                productsRows.Add(row);

            return productsRows;
        }

        public static List<ProductListingModel> FilterByPrice(this IProductService service, List<ProductListingModel> products, string? priceFilter)
        {
            if (priceFilter == "desc")
                products = products
                    .OrderByDescending(x => x)
                    .ToList();
            else if (priceFilter == "asc")
                products = products
                    .OrderBy(x => x)
                    .ToList();

            return products;
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
    }
}
