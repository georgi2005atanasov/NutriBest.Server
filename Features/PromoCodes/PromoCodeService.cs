namespace NutriBest.Server.Features.PromoCodes
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.PromoCodes.Extensions;
    using NutriBest.Server.Features.PromoCodes.Models;

    public class PromoCodeService : IPromoCodeService
    {
        private readonly NutriBestDbContext db;

        public PromoCodeService(NutriBestDbContext db)
        {
            this.db = db;
        }

        public async Task<List<string>> Create(decimal discountPercentage, 
            int count,
            string description)
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

                promoCode.DiscountPercentage = discountPercentage;
                promoCode.Description = description;

                db.PromoCodes.Add(promoCode);
                codes.Add(code);
            }

            await db.SaveChangesAsync();

            return codes;
        }

        public async Task<PromoCodeListingModel> GetByCode(string code)
        {
            var promoCode = await db.PromoCodes
                .Select(x => new PromoCodeListingModel
                {
                    DiscountPercentage = x.DiscountPercentage,
                    Code = x.Code
                })
                .FirstOrDefaultAsync(x => x.Code == code);

            if (promoCode == null)
                throw new ArgumentNullException("Invalid promo code!");

            return promoCode;
        }
    }
}
