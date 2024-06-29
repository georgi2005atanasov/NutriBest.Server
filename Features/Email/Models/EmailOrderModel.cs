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

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (EmailOrderModel)obj;
            return CustomerName == other.CustomerName &&
                   OrderId == other.OrderId &&
                   Subject == other.Subject &&
                   CustomerEmail == other.CustomerEmail &&
                   OrderDetailsUrl == other.OrderDetailsUrl &&
                   TotalPrice == other.TotalPrice && 
                   PhoneNumber == other.PhoneNumber;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CustomerName,
                OrderId,
                Subject, 
                CustomerEmail, 
                OrderDetailsUrl,
                TotalPrice,
                PhoneNumber);
        }
    }
}
