namespace NutriBest.Server.Features.Invoices.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.Invoice;

    public class InvoiceServiceModel
    {
        [Required]
        [MaxLength(FirstNameMaxLength)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(LastNameMaxLength)]
        public string LastName { get; set; } = null!;

        [Required]
        [StringLength(CompanyNameMaxLength, MinimumLength = CompanyNameMinLength)]
        public string CompanyName { get; set; } = null!;

        [Required]
        public string PhoneNumber { get; set; } = null!;

        public string? Bullstat { get; set; }

        public string? VAT { get; set; }

        [Required]
        [MaxLength(PersonInChargeMaxLength)]
        public string PersonInCharge { get; set; } = null!;
    }
}
