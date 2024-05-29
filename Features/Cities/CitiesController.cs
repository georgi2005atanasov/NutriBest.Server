namespace NutriBest.Server.Features.Cities
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Cities.Models;

    public class CitiesController : ApiController
    {
        private readonly NutriBestDbContext db;
        private readonly IMemoryCache memoryCache;

        public CitiesController(NutriBestDbContext db,
            IMemoryCache memoryCache)
        {
            this.db = db;
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<ActionResult<List<AllCitiesServiceModel>>> AllCitiesWithCountry()
        {
            try
            {
                const string cacheKey = "allCities";
                if (!memoryCache.TryGetValue(cacheKey, out List<AllCitiesServiceModel> cities))
                {
                    cities = await db.Cities
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

                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(30)
                    };

                    memoryCache.Set(cacheKey, cities, cacheEntryOptions);
                }

                return Ok(cities);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
