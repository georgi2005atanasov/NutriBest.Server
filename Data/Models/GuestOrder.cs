namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static Validation.GuestOrder;

    public class GuestOrder
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(EmailMaxLength)]
        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public int OrderId { get; set; }

        public Order? Order { get; set; }

        public bool IsDeleted { get; set; }
    }
}
