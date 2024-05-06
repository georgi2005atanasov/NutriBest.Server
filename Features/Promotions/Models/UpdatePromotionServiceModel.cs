﻿namespace NutriBest.Server.Features.Promotions.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.Promotion;

    public class UpdatePromotionServiceModel
    {
        [StringLength(MaxDescriptionLength, MinimumLength = MinDescriptionLength)]
        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? DiscountPercentage { get; set; }

        public string? DiscountAmount { get; set; }

        public string? MinimumPrice { get; set; }

        public string? Category { get; set; }

        public string? Brand { get; set; }
    }
}
