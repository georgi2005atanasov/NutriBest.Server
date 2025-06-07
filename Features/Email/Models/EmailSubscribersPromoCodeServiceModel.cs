namespace NutriBest.Server.Features.Email.Models
{
    public class EmailSubscribersPromoCodeServiceModel
    {
        public string Subject { get; set; } = null!;

        public string PromoCodeDescription { get; set; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (EmailSubscribersPromoCodeServiceModel)obj;
            return PromoCodeDescription == other.PromoCodeDescription &&
                   Subject == other.Subject;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PromoCodeDescription, Subject);
        }
    }
}
