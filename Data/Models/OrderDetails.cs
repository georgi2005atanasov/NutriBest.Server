namespace NutriBest.Server.Data.Models
{
    using NutriBest.Server.Data.Enums;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class OrderDetails
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [NotMapped]
        public Order Order { get; set; } = null!;

        public DateTime MadeOn { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public bool IsPaid { get; set; }

        [Required]
        public bool IsShipped { get; set; }

        [Required]
        public int AddressId { get; set; }

        public Address? Address { get; set; }

        public int CountryId { get; set; }

        public Country? Country { get; set; }

        public int? InvoiceId { get; set; }

        public Invoice? Invoice { get; set; }

        public bool IsDeleted { get; set; }
    }
}
