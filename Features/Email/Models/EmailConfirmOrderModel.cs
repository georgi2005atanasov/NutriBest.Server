namespace NutriBest.Server.Features.Email.Models
{
    using System.ComponentModel.DataAnnotations;

    public class EmailConfirmOrderModel : EmailModel
    {
        public int OrderId { get; set; }

        [Required]
        public string ConfirmationUrl { get; set; } = null!;

        public string CustomerName { get; set; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (EmailConfirmOrderModel)obj;
            return CustomerName == other.CustomerName &&
                   OrderId == other.OrderId &&
                   ConfirmationUrl == other.ConfirmationUrl &&
                   To == other.To &&
                   Subject == other.Subject;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CustomerName, OrderId, ConfirmationUrl, To, Subject);
        }
    }
}
