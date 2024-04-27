﻿namespace NutriBest.Server.Data.Models
{
    using NutriBest.Server.Data.Enums;
    using NutriBest.Server.Data.Models.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static Validation.Profile;

    public class Profile : DeletableEntity
    {
        [Key]
        [Required]
        public string UserId { get; set; } = null!;

        [MaxLength(NameMaxLength)]
        public string? Name { get; set; }

        public int? Age { get; set; }

        public Gender? Gender { get; set; }

        [NotMapped]
        public Address? Address { get; set; }

        public int? CartId { get; set; }

        [NotMapped]
        public Cart? Cart { get; set; } = new Cart();

        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
