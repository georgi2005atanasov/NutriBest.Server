namespace NutriBest.Server.Data.Models
{
    using NutriBest.Server.Data.Models.Base;
    using System.ComponentModel.DataAnnotations;
    using static Validation.Promotion;

    public class Promotion : DeletableEntity
    {
        [Key]
        [Required]
        public int PromotionId { get; set; }

        [Required]
        [StringLength(MaxDescriptionLength, MinimumLength = MinDescriptionLength)]
        public string? Description { get; set; }

        [Range(MinPercentage, MaxPercentage)]
        public decimal? DiscountPercentage { get; set; }

        [Range(MinPrice, MaxPrice)]
        public decimal? DiscountAmount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();

        public string? Category { get; set; }

        public string? Brand { get; set; }
    }
}
