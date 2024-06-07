namespace NutriBest.Server.Features.Email.Models
{
    public class SendPromoEmailModel : EmailModel
    {
        public string PromoCodeDescription { get; set; } = null!;
    }
}
