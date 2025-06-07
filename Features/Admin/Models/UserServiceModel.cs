namespace NutriBest.Server.Features.Admin.Models
{
    public class UserServiceModel
    {
        public string? Name { get; set; }

        public string UserName { get; set; } = null!;

        public int? Age { get; set; }

        public string? Gender { get; set; }

        public bool? IsDeleted { get; set; }

        public IList<string>? Roles { get; set; }
    }
}
