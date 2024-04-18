using NutriBest.Server.Data.Models;
using NutriBest.Server.Features.Products.Models;
using System.IO.Compression;
using System.Text;

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

        public static void CopyTo(this IProductService service, Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static byte[] Zip(this IProductService service, string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }

                return mso.ToArray();
            }
        }

        public static string Unzip(this IProductService service, byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        public static IQueryable<Product> OrderByCategories(this IProductService service, IQueryable<Product> query, string categoriesFilter = "")
        {
            if (!string.IsNullOrEmpty(categoriesFilter))
            {
                var categoriesToCheck = categoriesFilter
                    .Split()
                    .ToList();

                query = query
                    .Where(x => x.ProductsCategories
                                .Select(x => x.Category.Name)
                                .Any(x => categoriesToCheck.Contains(x)));
            }

            return query;
        }

        public static IQueryable<ProductListingModel> OrderByPrice(this IProductService service, IQueryable<ProductListingModel> queryProducts, string priceFilter = "")
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

        public static IQueryable<ProductListingModel> OrderByName(this IProductService service, IQueryable<ProductListingModel> queryProducts, string alphaFilter = "")
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
    }
}
