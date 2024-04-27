namespace NutriBest.Server.Data.Models
{
    public class Address
    {
        public int Id { get; set; }

        public string? ProfileId { get; set; }

        public Profile? Profile { get; set; }

        public string? Street { get; set; }

        public int? StreetNumber { get; set; }

        public string? Country { get; set; }

        public string? City { get; set; }

        public int? PostalCode { get; set; }
    }
}
