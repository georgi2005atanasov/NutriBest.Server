namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class CartProduct
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }

        public Cart? Cart { get; set; }

        [Required]
        public int ProductId { get; set; }

        public int Count { get; set; }
    }
}
