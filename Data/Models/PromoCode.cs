namespace NutriBest.Server.Data.Models
{
    using NutriBest.Server.Data.Models.Base;
    using System.ComponentModel.DataAnnotations;

    public class PromoCode : DeletableEntity
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public string Code { get; set; } = null!;

        [Required]
        public decimal DiscountPercentage { get; set; }

        public bool IsValid { get; set; }
    }
}
