namespace NutriBest.Server.Features.Carts.Models
{
    using NutriBest.Server.Features.Images.Models;
    using NutriBest.Server.Features.Products.Models;

    public class CartProductServiceModel
    {
        public int ProductId { get; set; }

        public ProductServiceModel? Product { get; set; }

        public int Count { get; set; }
    }
}
