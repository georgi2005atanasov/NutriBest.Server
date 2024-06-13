namespace NutriBest.Server.Features.Orders.Models
{
    public class LowInStockServiceModel
    {
        public string Name { get; set; } = null!;

        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
