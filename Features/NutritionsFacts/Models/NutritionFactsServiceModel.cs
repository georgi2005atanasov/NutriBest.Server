using NutriBest.Server.Data;
using System.ComponentModel.DataAnnotations;

namespace NutriBest.Server.Features.NutritionsFacts.Models
{
    using static Validation.NutritionFacts;
    public class NutritionFactsServiceModel
    {
        public int ProductId { get; set; }

        [Range(MinAmount, 50000)]
        public double? EnergyValue { get; set; }

        [Range(MinAmount, 50000)]
        public double? Fats { get; set; }

        [Range(MinAmount, 50000)]
        public double? SaturatedFats { get; set; }

        [Range(MinAmount, 50000)]
        public double? Carbohydrates { get; set; }

        [Range(MinAmount, 50000)]
        public double? Sugars { get; set; }

        [Range(MinAmount, 50000)]
        public double? Proteins { get; set; }

        [Range(MinAmount, 50000)]
        public double? Salt { get; set; }
    }
}
