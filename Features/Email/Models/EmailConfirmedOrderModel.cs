namespace NutriBest.Server.Features.Email.Models
{
    public class EmailConfirmedOrderModel
    {
        public int OrderId { get; set; }

        public string? Subject { get; set; }

        public string OrderDetailsUrl { get; set; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (EmailConfirmedOrderModel)obj;
            return OrderId == other.OrderId &&
                   OrderDetailsUrl == other.OrderDetailsUrl &&
                   Subject == other.Subject;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OrderId, Subject, OrderDetailsUrl);
        }
    }
}
