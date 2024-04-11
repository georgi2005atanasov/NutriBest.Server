﻿namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ProductImage
    {
        public int ProductImageId { get; set; }

        [Required]
        public string ImageData { get; set; } = null!;

        [Required]
        public string ContentType { get; set; } = null!;

        [NotMapped]
        public int ProductId { get; set; }

        [NotMapped]
        public Product Product { get; set; } = null!;
    }
}
