namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Cart
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [NotMapped]
        public Profile? Profile { get; set; }

        [Required]
        public List<CartProduct> CartProducts { get; set; } = new List<CartProduct>();

        [Required]
        public decimal TotalPrice { get; set; }
    }
}
