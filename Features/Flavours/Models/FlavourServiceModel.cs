namespace NutriBest.Server.Features.Flavours.Models
{
    using System.ComponentModel.DataAnnotations;

    public class FlavourServiceModel
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
