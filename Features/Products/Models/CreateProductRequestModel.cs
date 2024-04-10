﻿using System.ComponentModel.DataAnnotations;

namespace NutriBest.Server.Features.Products.Models
{
    public class CreateProductRequestModel
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public List<string> Categories { get; set; } = null!;

        [Required]
        public IFormFile Image { get; set; } = null!;
    }
}
