
namespace NutriBest.Server.Features.NutritionsFacts.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.NutritionFacts;

    public class NutritionFactsServiceModel
    {
        [Range(MinAmount, MaxAmount)]
        public string? EnergyValue { get; set; }

        [Range(MinAmount, MaxAmount)]
        public string? Fats { get; set; }

        [Range(MinAmount, MaxAmount)]
        public string? SaturatedFats { get; set; }

        [Range(MinAmount, MaxAmount)]
        public string? Carbohydrates { get; set; }

        [Range(MinAmount, 50000)]
        public string? Sugars { get; set; }

        [Range(MinAmount, MaxAmount)]
        public string? Proteins { get; set; }

        [Range(MinAmount, MaxAmount)]
        public string? Salt { get; set; }
    }
}
