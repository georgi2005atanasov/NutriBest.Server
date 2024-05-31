﻿namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CartProduct
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }

        public Cart? Cart { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        [Required]
        public int FlavourId { get; set; }

        public Flavour? Flavour { get; set; }

        [Required]
        public int PackageId { get; set; }

        public Package? Package { get; set; }

        public int Count { get; set; }
    }
}
