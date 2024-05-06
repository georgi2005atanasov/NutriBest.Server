﻿namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static Validation.Brand;

    public class Brand
    {
        public int Id { get; set; }

        [Required]
        [StringLength(MaxBrandLength)]
        public string Name { get; set; } = null!;

        [StringLength(MaxBrandDescriptionLength)]
        public string? Description { get; set; }

        public int BrandLogoId { get; set; }

        [NotMapped]
        public BrandLogo? BrandLogo { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
    }
}
