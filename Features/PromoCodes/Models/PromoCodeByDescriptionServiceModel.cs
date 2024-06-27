namespace NutriBest.Server.Features.PromoCodes.Models
{
    public class PromoCodeByDescriptionServiceModel
    {
        public int ExpireIn { get; set; } // days

        public string Description { get; set; } = null!;

        public List<string>? PromoCodes { get; set; }
    }
}
