namespace NutriBest.Server.Features.Products.Models
{
    public class AllProductsModel
    {
        public IEnumerable<IEnumerable<ProductListingModel>>? ProductsRows { get; set; }

        public int Count { get; set; }
    }
}
