namespace NutriBest.Server.Features.Orders.Models
{
    using NutriBest.Server.Features.Products.Models;

    public class OrderRelatedProductsServiceModel
    {
        public List<OrderRelatedProductServiceModel> Products { get; set; } = new List<OrderRelatedProductServiceModel>();
    }
}
