namespace NutriBest.Server.Features.Carts.Models
{
    using NutriBest.Server.Features.Products.Models;

    public class CartProductServiceModel
    {
        public int ProductId { get; set; }

        public ProductListingServiceModel? Product { get; set; }

        public int Count { get; set; }

        public string Flavour { get; set; } = null!;

        public int Grams { get; set; }

        public decimal? Price { get; set; }
    }
}
