namespace NutriBest.Server.Features.ShippingDiscounts
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.ShippingDiscounts.Models;

    public class ShippingDiscountService : IShippingDiscountService
    {
        private readonly NutriBestDbContext db;

        public ShippingDiscountService(NutriBestDbContext db)
        {
            this.db = db;
        }

        public async Task<AllShippingDiscountsServiceModel> All()
        {
            var countries = db.Countries
                .AsQueryable();

            var allShippingDiscounts = new AllShippingDiscountsServiceModel();

            foreach (var country in countries)
            {
                if (country.ShippingDiscountId != null)
                {
                    var discount = await Get(country.CountryName);
                    allShippingDiscounts.ShippingDiscounts.Add(discount);
                }
            }

            return allShippingDiscounts;
        }

        public async Task<ShippingDiscountServiceModel> Get(string countryName)
        {
            var country = await GetCountry(countryName);

            if (country == null)
                throw new ArgumentNullException("Invalid country!");

            var shippingDiscount = await db.ShippingDiscounts
                .FirstOrDefaultAsync(x => x.Id == country.ShippingDiscountId);

            if (shippingDiscount == null)
                throw new ArgumentNullException($"There is no shipping discount for {countryName}!");

            var shippingDiscountModel = new ShippingDiscountServiceModel
            {
                Description = shippingDiscount.Description,
                DiscountPercentage = shippingDiscount.DiscountPercentage,
                CountryName = country.CountryName,
                EndDate = shippingDiscount.EndDate,
                MinimumPrice = shippingDiscount.MinimumPrice
            };

            return shippingDiscountModel;
        }

        public async Task<int> Create(string countryName,
            decimal discountPercentage,
            DateTime? endDate,
            string description,
            string? minimumPrice)
        {
            var country = await GetCountry(countryName);

            if (country == null)
                throw new ArgumentNullException("Invalid country!");

            if (await db.ShippingDiscounts.AnyAsync(x => x.Id == country.ShippingDiscountId))
                throw new InvalidOperationException($"{countryName} already has a shipping discount!");

            if (!string.IsNullOrEmpty(minimumPrice))
            {
                var minPrice = decimal.Parse(minimumPrice);

                var shippingDiscount = new ShippingDiscount
                {
                    Description = description,
                    DiscountPercentage = discountPercentage,
                    EndDate = endDate,
                    MinimumPrice = minPrice
                };

                db.ShippingDiscounts.Add(shippingDiscount);

                await db.SaveChangesAsync();

                country.ShippingDiscountId = shippingDiscount.Id;

                await db.SaveChangesAsync();

                return shippingDiscount.Id;
            }
            else
            {
                var shippingDiscount = new ShippingDiscount
                {
                    Description = description,
                    DiscountPercentage = discountPercentage,
                    EndDate = endDate
                };

                db.ShippingDiscounts.Add(shippingDiscount);

                await db.SaveChangesAsync();

                country.ShippingDiscountId = shippingDiscount.Id;

                await db.SaveChangesAsync();

                return shippingDiscount.Id;
            }
        }

        public async Task<bool> Delete(string countryName)
        {
            var country = await GetCountry(countryName);

            if (country == null)
                throw new ArgumentNullException("Invalid country!");

            var shippingDiscount = await db.ShippingDiscounts
                .FirstOrDefaultAsync(x => x.Id == country.ShippingDiscountId);

            if (shippingDiscount == null)
                return false;

            country.ShippingDiscountId = null;
            db.ShippingDiscounts.Remove(shippingDiscount);

            await db.SaveChangesAsync();

            return true;
        }

        private async Task<Country?> GetCountry(string countryName)
            => await db.Countries
            .FirstOrDefaultAsync(x => x.CountryName == countryName);
    }
}
