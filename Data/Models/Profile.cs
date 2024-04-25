namespace NutriBest.Server.Data.Models
{
    using NutriBest.Server.Data.Enums;
    using System.ComponentModel.DataAnnotations;
    using static Validation.Profile;

    public class Profile
    {
        [Key]
        [Required]
        public string UserId { get; set; } = null!;

        [MaxLength(NameMaxLength)]
        public string? Name { get; set; }

        public Gender? Gender { get; set; }

        public int? Age { get; set; }
    }
}
