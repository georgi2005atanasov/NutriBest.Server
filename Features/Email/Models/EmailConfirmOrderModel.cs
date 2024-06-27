namespace NutriBest.Server.Features.Email.Models
{
    using System.ComponentModel.DataAnnotations;

    public class EmailConfirmOrderModel : EmailModel
    {
        public int OrderId { get; set; }

        [Required]
        public string ConfirmationUrl { get; set; } = null!;

        public string CustomerName { get; set; } = null!;
    }
}
