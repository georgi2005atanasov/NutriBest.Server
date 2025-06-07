namespace NutriBest.Server.Data.Models
{
    using NutriBest.Server.Data.Models.Base;

    public class Newsletter : DeletableEntity
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public bool IsAnonymous { get; set; }

        public bool HasOrders { get; set; }

        public int TotalOrders { get; set; }

        public string? Name { get; set; }

        public string? PhoneNumber { get; set; }

        public string? VerificationToken { get; set; }
    }
}
