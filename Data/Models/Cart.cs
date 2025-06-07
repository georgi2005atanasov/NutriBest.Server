namespace NutriBest.Server.Data.Models
{
    using NutriBest.Server.Data.Models.Base;
    using System.ComponentModel.DataAnnotations;

    public class Cart : Entity
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public Profile? Profile { get; set; }

        [Required]
        public List<CartProduct> CartProducts { get; set; } = new List<CartProduct>();

        public string? Code { get; set; }

        [Required]
        public decimal TotalProducts { get; set; }

        [Required]
        public decimal TotalSaved { get; set; }

        [Required]
        public decimal OriginalPrice { get; set; }

        public decimal? ShippingPrice { get; set; }
    }
}
