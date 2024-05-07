namespace NutriBest.Server.Features.Brands
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Brands.Models;

    public class BrandsController : ApiController
    {
        private readonly NutriBestDbContext db;

        public BrandsController(NutriBestDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<ActionResult<BrandServiceModel>> All()
        {
            try
            {
                var brands = await db.Brands
                    .Select(x => new BrandServiceModel
                    {
                        Name = x.Name,
                        BrandLogoId = x.BrandLogoId
                    })
                    .ToListAsync();

                return Ok(brands);
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    Message = "Could not get the brands!"
                });
            }
        }
    }
}
