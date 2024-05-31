namespace NutriBest.Server.Features.Orders.Models
{
    using NutriBest.Server.Features.Carts.Models;
    using NutriBest.Server.Features.Invoices.Models;

    public class OrderServiceModel
    {
        public CartServiceModel? Cart { get; set; }

        public bool IsFinished { get; set; }

        public bool IsConfirmed { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public InvoiceServiceModel? Invoice { get; set; }

        public DateTime MadeOn { get; set; }
    }
}
