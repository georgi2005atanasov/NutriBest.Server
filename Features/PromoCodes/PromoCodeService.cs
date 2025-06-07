using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.PromoCodes
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.PromoCodes.Extensions;
    using NutriBest.Server.Features.PromoCodes.Models;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;
    using static ErrorMessages.PromoCodeController;

    public class PromoCodeService : IPromoCodeService, ITransientService
    {
        private readonly NutriBestDbContext db;

        public PromoCodeService(NutriBestDbContext db)
        {
            this.db = db;
        }

        public async Task<List<PromoCodeByDescriptionServiceModel>> All()
            => await db.PromoCodes
                .GroupBy(x => x.Description)
                .Select(x => new PromoCodeByDescriptionServiceModel
                {
                    Description = x.Key,
                    ExpireIn = 10 - x
                        .Select(y =>
                                    (y.CreatedOn - DateTime.UtcNow)
                                    .Duration()
                                    .Days)
                        .FirstOrDefault(),
                    PromoCodes = db
                        .PromoCodes
                        .Where(y => y.Description == x.Key)
                        .Select(x => x.Code)
                        .ToList()
                })
                .ToListAsync();

        public async Task<List<string>> Create(decimal discountPercentage,
            int count,
            string description)
        {
            var codes = new List<string>();

            for (int i = 0; i < count; i++)
            {
                string code = PromoCodeGenerator.GeneratePromoCode(7);

                if (await db.PromoCodes.AnyAsync(x => x.Code == code))
                {
                    i--; // low chance
                    continue;
                }

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

        public async Task<bool> DisableByCode(string code)
        {
            var promoCode = await db.PromoCodes
                .FirstOrDefaultAsync(x => x.Code == code);

            if (promoCode == null)
                throw new InvalidOperationException(InvalidPromoCode);

            db.PromoCodes.Remove(promoCode);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DisableByDescription(string description)
        {
            var promoCodes = db.PromoCodes
                .Where(x => x.Description == description);

            db.PromoCodes.RemoveRange(promoCodes);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<PromoCodeListingModel> GetByCode(string code)
        {
            var promoCode = await db.PromoCodes
                .Select(x => new PromoCodeListingModel
                {
                    DiscountPercentage = x.DiscountPercentage,
                    Code = x.Code,
                })
                .FirstOrDefaultAsync(x => x.Code == code);

            if (promoCode == null)
                throw new ArgumentNullException(InvalidPromoCode);

            return promoCode;
        }

        public async Task<(List<string>, int)> GetByDescription(string description)
        {
            var promoCodes = db.PromoCodes
                .Where(x => x.Description == description && !x.IsSent)
                .Select(x => new
                {
                    x.Code,
                    x.CreatedOn
                });

            if (!await promoCodes.AnyAsync())
            {
                return (new List<string>(), 0);
            }

            var expireIn = 10 - (promoCodes.First().CreatedOn - DateTime.UtcNow).Duration().Days;

            return (await promoCodes.Select(x => x.Code).ToListAsync(), expireIn);
        }
    }
}
