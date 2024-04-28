namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ProductPromotion
    {
        [Key]
        [Required]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        [Key]
        [Required]
        public int PromotionId { get; set; }

        public Promotion? Promotion { get; set; }

        public decimal? SpecialPrice { get; set; }

        public bool IsDeleted { get; set; }
    }
}
