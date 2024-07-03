using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.ShippingDiscounts
{
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.ShippingDiscounts.Models;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;
    using static ErrorMessages.ShippingDiscountController;

    public class ShippingDiscountService : IShippingDiscountService, ITransientService
    {
        private readonly NutriBestDbContext db;

        public ShippingDiscountService(NutriBestDbContext db) 
            => this.db = db;

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
                throw new ArgumentNullException(InvalidCountry);

            var shippingDiscount = await db.ShippingDiscounts
                .FirstOrDefaultAsync(x => x.Id == country.ShippingDiscountId);

            if (shippingDiscount == null)
                throw new ArgumentNullException(string.Format(ShippingDiscountDoesNotExists, countryName));

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
                throw new ArgumentNullException(InvalidCountry);

            if (await db.ShippingDiscounts.AnyAsync(x => x.Id == country.ShippingDiscountId))
                throw new InvalidOperationException(string.Format(CountryAlreadyHasShippingDiscount, countryName));

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

        public async Task<bool> Remove(string countryName)
        {
            var country = await GetCountry(countryName);

            if (country == null)
                throw new ArgumentNullException(InvalidCountry);

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
