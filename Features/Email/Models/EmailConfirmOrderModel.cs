namespace NutriBest.Server.Features.Email.Models
{
    using System.ComponentModel.DataAnnotations;

    public class EmailConfirmOrderModel : EmailModel
    {
        [Required]
        public string ConfirmationUrl { get; set; } = null!;

        public int OrderId { get; set; }

        public string CustomerName { get; set; } = null!;
    }
}
