namespace NutriBest.Server.Data.Models
{
    using NutriBest.Server.Data.Enums;
    using NutriBest.Server.Data.Models.Base;
    using System.ComponentModel.DataAnnotations;
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
    }
}
