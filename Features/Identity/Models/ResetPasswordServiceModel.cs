namespace NutriBest.Server.Features.Identity.Models
{
    public class ResetPasswordServiceModel
    {
        public string NewPassword { get; set; } = null!;

        public string ConfirmPassword { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Token { get; set; } = null!;
    }
}
