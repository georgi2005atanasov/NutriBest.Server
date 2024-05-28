namespace NutriBest.Server.Features.Cities
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
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
                    .GroupBy(c => c.Country.CountryName)
                    .Select(x => new AllCitiesServiceModel
                    {
                        Country = x.Key, // be aware
                        Cities = x.Select(y => new CityServiceModel
                        {
                            CityName = y.CityName,
                            PostalCode = y.PostalCode
                        })
                        .ToList()
                    })
                    .ToListAsync();

                return Ok(cities);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
