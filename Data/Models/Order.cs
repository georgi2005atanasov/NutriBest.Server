using NutriBest.Server.Data.Enums;
using NutriBest.Server.Data.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace NutriBest.Server.Data.Models
{
    public class Order : DeletableEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string ProfileId { get; set; } = null!;

        public Profile? Profile { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public bool IsPaid { get; set; }

        public bool IsShipped { get; set; }

        [Required]
        public int CartId { get; set; }

        public Cart? Cart { get; set; }
    }
}
