namespace NutriBest.Server.Features.Identity.Models
{
    using System.ComponentModel.DataAnnotations;

    public class LoginServiceModel
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
