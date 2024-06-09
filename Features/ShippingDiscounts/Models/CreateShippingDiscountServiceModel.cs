namespace NutriBest.Server.Features.ShippingDiscounts.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.Promotion;

    public class CreateShippingDiscountServiceModel
    {
        public string CountryName { get; set; } = null!;

        public string DiscountPercentage { get; set; } = null!;

        public string? MinimumPrice { get; set; }

        [StringLength(MaxDescriptionLength, MinimumLength = MinDescriptionLength)]
        public string Description { get; set; } = null!;

        public DateTime? EndDate { get; set; }
    }
}
