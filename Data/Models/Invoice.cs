namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static Validation.Invoice;

    public class Invoice
    {
        [Key]
        [Required]
        public int Id { get; set; }

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

        public OrderDetails? OrderDetails { get; set; }

        public bool IsDeleted { get; set; }
    }
}
