﻿namespace NutriBest.Server.Features.Cities
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Cities.Models;

    public class CitiesController : ApiController
    {
        private readonly NutriBestDbContext db;

        public CitiesController(NutriBestDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<AllCitiesServiceModel>>> AllCitiesWithCountry()
        {
            try
            {
                var cities = await db.Cities
                        .Include(c => c.Country)
                        .GroupBy(c => c.Country!.CountryName) // be aware
                        .Select(x => new AllCitiesServiceModel
                        {
                            Country = x.Key,
                            Cities = x.Select(y => new CityServiceModel
                            {
                                CityName = y.CityName,
                                PostalCode = y.PostalCode
                            })
                            .OrderBy(x => x.CityName)
                            .ToList()
                        })
                        .OrderBy(x => x.Country)
                        .ToListAsync();

                foreach (var data in cities)
                {
                    var countryFromDb = await db.Countries
                        .FirstAsync(x => x.CountryName == data.Country);

                    data.ShippingPrice = countryFromDb.ShippingPrice;

                    if (countryFromDb.ShippingDiscountId != null)
                    {
                        var shippingDiscount = await db.ShippingDiscounts
                            .FirstAsync(x => x.Id == countryFromDb.ShippingDiscountId);

                        data.MinimumPriceForDiscount = shippingDiscount.MinimumPrice;
                        data.ShippingPriceWithDiscount = data.ShippingPrice * ((100 - shippingDiscount.DiscountPercentage) / 100);
                    }
                    else
                    {
                        data.ShippingPriceWithDiscount = data.ShippingPrice;
                    }
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                };

                return Ok(cities);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}