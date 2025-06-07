namespace NutriBest.Server.Features.Orders.Models
{
    public class LowInStockServiceModel
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = null!;

        public int Quantity { get; set; }
    }
}
