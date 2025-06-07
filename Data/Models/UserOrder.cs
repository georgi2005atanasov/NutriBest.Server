namespace NutriBest.Server.Data.Models
{
    using NutriBest.Server.Data.Models.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class UserOrder : DeletableEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string ProfileId { get; set; } = null!;

        public Profile? Profile { get; set; }

        public int OrderId { get; set; }

        public Order? Order { get; set; }
    }
}
