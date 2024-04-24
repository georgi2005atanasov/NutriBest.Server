namespace NutriBest.Server.Features.Identity.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UserServiceModel
    {
        public string? Name { get; set; }

        public string? Gender { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
