namespace NutriBest.Server.Data.Models
{
    using NutriBest.Server.Data.Models.Base;
    using System.ComponentModel.DataAnnotations;
    using static Validation.Promotion;

    public class ShippingDiscount : DeletableEntity
    {
        public int Id { get; set; }

        public List<Country> Countries { get; set; } = new List<Country>();

        [Range(MinPercentage, MaxPercentage)]
        public decimal DiscountPercentage { get; set; }

        public decimal? MinimumPrice { get; set; }

        [StringLength(MaxDescriptionLength, MinimumLength = MinDescriptionLength)]
        public string Description { get; set; } = null!;

        public DateTime? EndDate { get; set; }
    }
}
