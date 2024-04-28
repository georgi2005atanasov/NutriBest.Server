namespace NutriBest.Server.Data.Models
{
    using NutriBest.Server.Data.Enums;
    using System.ComponentModel.DataAnnotations;

    public class OrderDetails
    {
        [Key]
        [Required]
        public int OrderId { get; set; }

        public Order? Order { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public bool IsPaid { get; set; }

        [Required]
        public bool IsShipped { get; set; }

        [Required]
        public int AddressId { get; set; }

        public Address? Address { get; set; }

        public bool IsDeleted { get; set; }
    }
}
