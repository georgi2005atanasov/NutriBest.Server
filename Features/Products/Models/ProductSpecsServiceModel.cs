namespace NutriBest.Server.Features.Products.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;
    using static ServicesConstants.Package;

    public class ProductSpecsServiceModel
    {
        [Required]
        [JsonPropertyName("flavour")]
        public string Flavour { get; set; } = null!;

        [Required]
        [Range(MinSize, MaxSize)]
        [JsonPropertyName("grams")]
        public int Grams { get; set; }

        [Required]
        [Range(MinQuantity, MaxQuantity)]
        [JsonPropertyName("quantity")]
        public int Quantity{ get; set; }

        [Required]
        [JsonPropertyName("price")]
        public string Price { get; set; } = null!;
    }
}
