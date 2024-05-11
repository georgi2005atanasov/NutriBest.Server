﻿namespace NutriBest.Server.Features.Products.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ServicesConstants.Product;
    using static ServicesConstants.Brand;

    public class UpdateProductServiceModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Price { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        [StringLength(MaxBrandLength)]
        public string Brand { get; set; } = null!;

        [Required]
        public List<string> Categories { get; set; } = null!;

        [Required]
        public List<ProductSpecsServiceModel> ProductSpecs { get; set; } = new List<ProductSpecsServiceModel>();

        public IFormFile? Image { get; set; }

        //more...
    }
}
