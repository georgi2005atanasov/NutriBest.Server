namespace NutriBest.Server.Features.PromoCodes
{
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.PromoCodes.Extensions;

    public class PromoCodeService : IPromoCodeService
    {
        private readonly NutriBestDbContext db;

        public PromoCodeService(NutriBestDbContext db)
        {
            this.db = db;
        }

        public async Task<List<string>> Create(decimal? discountAmount, decimal? discountPercentage, int count)
        {
            var codes = new List<string>();

            for (int i = 0; i < count; i++)
            {
                string code = PromoCodeGenerator.GeneratePromoCode(7);

                var promoCode = new PromoCode
                {
                    Code = code,
                    IsValid = true // set by default
                };

                if (discountAmount != null)
                    promoCode.DiscountAmount = discountAmount;

                if (discountPercentage != null)
                    promoCode.DiscountPercentage = discountPercentage;

                db.PromoCodes.Add(promoCode);
                codes.Add(code);
            }

            await db.SaveChangesAsync();

            return codes;
        }
    }
}
