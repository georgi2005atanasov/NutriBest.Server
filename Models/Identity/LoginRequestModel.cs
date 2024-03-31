using System.ComponentModel.DataAnnotations;

namespace NutriBest.Server.Data.Models.Identity
{
    public class LoginRequestModel
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
