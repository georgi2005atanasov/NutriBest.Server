using System.ComponentModel.DataAnnotations;

namespace NutriBest.Server.Features.Identity.Models
{
    public class RegisterRequestModel
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

    }
}
