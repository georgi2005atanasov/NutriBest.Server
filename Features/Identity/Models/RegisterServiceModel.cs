namespace NutriBest.Server.Features.Identity.Models
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterServiceModel
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public string ConfirmPassword { get; set; } = null!;

    }
}
