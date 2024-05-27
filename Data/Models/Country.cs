﻿namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static Validation.Country;
    public class Country
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(CountryNameMaxLength)]
        public string CountryName { get; set; } = null!;

        public HashSet<Address> Addresses { get; set; } = new HashSet<Address>();
    }
}
