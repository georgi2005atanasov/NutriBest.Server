﻿namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static Validation.NutritionFacts;

    public class NutritionFacts
    {
        [Key]
        [Required]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

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

        public bool IsDeleted { get; set; }
    }
}