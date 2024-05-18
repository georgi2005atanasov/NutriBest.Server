namespace NutriBest.Server.Features.Carts.Models
{
    using NutriBest.Server.Features.Products.Models;

    public class CartProductServiceModel
    {
        public int ProductId { get; set; }

        public ProductListingServiceModel? Product { get; set; }

        public int Count { get; set; }
    }
}
