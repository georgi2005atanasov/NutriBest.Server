namespace NutriBest.Server.Features.Email.Models
{
    public class EmailOrderModel
    {
        public string CustomerName { get; set; } = null!;

        public string CustomerEmail { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Subject { get; set; }

        public int OrderId { get; set; }

        public string TotalPrice { get; set; } = null!;

        public string OrderDetailsUrl { get; set; } = null!;
    }
}
