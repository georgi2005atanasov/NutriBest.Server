
namespace NutriBest.Server.Features.NutritionsFacts.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.NutritionFacts;

    public class NutritionFactsServiceModel
    {
        public int ProductId { get; set; }

        [Range(MinAmount, MaxAmount)]
        public double? EnergyValue { get; set; }

        [Range(MinAmount, MaxAmount)]
        public double? Fats { get; set; }

        [Range(MinAmount, MaxAmount)]
        public double? SaturatedFats { get; set; }

        [Range(MinAmount, MaxAmount)]
        public double? Carbohydrates { get; set; }

        [Range(MinAmount, 50000)]
        public double? Sugars { get; set; }

        [Range(MinAmount, MaxAmount)]
        public double? Proteins { get; set; }

        [Range(MinAmount, MaxAmount)]
        public double? Salt { get; set; }
    }
}
