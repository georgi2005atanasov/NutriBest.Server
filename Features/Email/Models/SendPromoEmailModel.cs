namespace NutriBest.Server.Features.Email.Models
{
    public class SendPromoEmailModel : EmailModel
    {
        public string PromoCodeDescription { get; set; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (SendPromoEmailModel)obj;
            return PromoCodeDescription == other.PromoCodeDescription &&
                   To == other.To &&
                   Subject == other.Subject;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PromoCodeDescription, Subject, To);
        }
    }
}
