namespace NutriBest.Server.Features.Orders.Models
{
    public class SummaryServiceModel
    {
        public int TotalOrders { get; set; }

        public int TotalProducts { get; set; }

        public decimal TotalPriceWithoutDiscount { get; set; }

        public decimal TotalDiscounts { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
