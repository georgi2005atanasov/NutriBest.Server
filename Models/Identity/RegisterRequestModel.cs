using System.ComponentModel.DataAnnotations;

namespace NutriBest.Server.Data.Models.Identity
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
