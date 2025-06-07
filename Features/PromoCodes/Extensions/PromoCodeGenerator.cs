namespace NutriBest.Server.Features.PromoCodes.Extensions
{
    using System.Text;

    public class PromoCodeGenerator
    {
        private static readonly Random random = new Random();
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string GeneratePromoCode(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentException("Length must be a positive integer", nameof(length));
            }

            StringBuilder promoCode = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                promoCode.Append(chars[random.Next(chars.Length)]);
            }

            return promoCode.ToString();
        }
    }
}
