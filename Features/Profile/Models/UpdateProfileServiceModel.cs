using NutriBest.Server.Data;

namespace NutriBest.Server.Features.Profile.Models
{
    using System.ComponentModel.DataAnnotations;
    using static Validation.Profile;

    public class UpdateProfileServiceModel
    {
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(NameMaxLength)]
        public string? UserName { get; set; }

        [Range(MinAge, MaxAge)]
        public int? Age { get; set; }

        public string? Name { get; set; }

        public string? Gender { get; set; }
    }
}