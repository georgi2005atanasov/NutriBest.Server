namespace NutriBest.Server.Features.PromoCodes.Models
{
    public class PromoCodeByDescriptionServiceModel
    {
        public List<string>? PromoCodes { get; set; }

        public int ExpireIn { get; set; } // days

        public string Description { get; set; } = null!;
    }
}
