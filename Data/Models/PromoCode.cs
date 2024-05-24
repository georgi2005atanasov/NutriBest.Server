namespace NutriBest.Server.Data.Models
{
    using NutriBest.Server.Data.Models.Base;
    using System.ComponentModel.DataAnnotations;

    public class PromoCode : DeletableEntity
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; } = null!;

        public decimal? DiscountPercentage { get; set; }

        public decimal? DiscountAmount { get; set; }

        public bool IsValid { get; set; }
    }
}
