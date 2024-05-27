namespace NutriBest.Server.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Country
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string CountryName { get; set; } = null!;
    }
}
