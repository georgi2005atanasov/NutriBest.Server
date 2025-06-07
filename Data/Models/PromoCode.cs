namespace NutriBest.Server.Data.Models
{
    using NutriBest.Server.Data.Models.Base;
    using System.ComponentModel.DataAnnotations;
    using static Validation.PromoCodes;

    public class PromoCode : DeletableEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(MaxDescriptionLength, MinimumLength = MinDescriptionLength)]
        public string Description { get; set; } = null!;

        [Required]
        public string Code { get; set; } = null!;

        [Required]
        [Range(MinDiscount, MaxDiscount)]
        public decimal DiscountPercentage { get; set; }

        public bool IsValid { get; set; }

        public bool IsSent { get; set; }
    }
}
