namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Promotion
    {
        [Key]
        [Required]
        public int PromotionId { get; set; }

        public string? Description { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public decimal? DiscountAmount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public List<ProductPromotion> ProductPromotions { get; set; } = new List<ProductPromotion>();
    }
}
