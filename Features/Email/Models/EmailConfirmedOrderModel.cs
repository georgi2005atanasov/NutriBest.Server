namespace NutriBest.Server.Features.Email.Models
{
    public class EmailConfirmedOrderModel
    {
        public int OrderId { get; set; }

        public string? Subject { get; set; }

        public string OrderDetailsUrl { get; set; } = null!;
    }
}
