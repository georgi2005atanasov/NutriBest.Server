namespace NutriBest.Server.Features.Products.Models
{
    public class AllProductsServiceModel
    {
        public int Count { get; set; }

        public int MaxPrice { get; set; }

        public IEnumerable<IEnumerable<ProductListingServiceModel>>? ProductsRows { get; set; }
    }
}
