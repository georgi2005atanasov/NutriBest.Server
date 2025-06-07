namespace NutriBest.Server.Features.Orders.Models
{
    using NutriBest.Server.Features.Carts.Models;
    using NutriBest.Server.Features.Invoices.Models;

    public class OrderServiceModel
    {
        public string City { get; set; } = null!;

        public string Country { get; set; } = null!;

        public string Street { get; set; } = null!;

        public string? StreetNumber { get; set; }

        public string? PhoneNumber { get; set; }

        public CartServiceModel? Cart { get; set; }

        public bool IsFinished { get; set; }

        public bool IsConfirmed { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public InvoiceServiceModel? Invoice { get; set; }

        public DateTime MadeOn { get; set; }

        public bool IsPaid { get; set; }

        public bool IsShipped { get; set; }

        public string Email { get; set; } = null!;

        public string CustomerName { get; set; } = null!;

        public string IBAN { get; set; } = null!;

        public decimal ShippingPrice { get; set; }

        public string? Comment { get; set; }
    }
}
