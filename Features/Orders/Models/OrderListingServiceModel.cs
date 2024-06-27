namespace NutriBest.Server.Features.Orders.Models
{
    public class OrderListingServiceModel
    {
        public int OrderId { get; set; }

        public bool IsFinished { get; set; }

        public bool IsConfirmed { get; set; }

        public DateTime MadeOn { get; set; }

        public bool IsShipped { get; set; }

        public bool IsPaid { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public string CustomerName { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string Email { get; set; } = null!;

        public bool IsAnonymous { get; set; }

        public string City { get; set; } = null!;

        public string Country { get; set; } = null!;

        public decimal TotalPrice { get; set; }
    }
}
