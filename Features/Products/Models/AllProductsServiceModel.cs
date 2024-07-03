namespace NutriBest.Server.Features.Products.Models
{
    public class AllProductsServiceModel
    {
        public int Count { get; set; }

        public int MaxPrice { get; set; }

        public List<List<ProductListingServiceModel>>? ProductsRows { get; set; }
    }
}
