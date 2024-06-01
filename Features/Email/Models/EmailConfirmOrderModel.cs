namespace NutriBest.Server.Features.Email.Models
{
    public class EmailConfirmOrderModel : EmailModel
    {
        public string ConfirmationUrl { get; set; } = null!;

        public int OrderId { get; set; }

        public string CustomerName { get; set; } = null!;
    }
}
