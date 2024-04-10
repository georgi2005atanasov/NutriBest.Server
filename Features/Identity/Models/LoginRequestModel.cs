namespace NutriBest.Server.Features.Identity.Models
{
    using System.ComponentModel.DataAnnotations;

    public class LoginRequestModel
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
