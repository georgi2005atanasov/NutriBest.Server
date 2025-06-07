namespace NutriBest.Server.Features.Newsletter.Models
{
    public class SubscriberServiceModel
    {
        public string? Name { get; set; }

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public bool IsAnonymous { get; set; }

        public bool HasOrders { get; set; }

        public int TotalOrders { get; set; }

        public DateTime RegisteredOn { get; set; }
    }
}
