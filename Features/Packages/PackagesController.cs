namespace NutriBest.Server.Features.Packages
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Packages.Models;

    public class PackagesController : ApiController
    {
        private readonly NutriBestDbContext db;
        private readonly IPackageService packageService;

        public PackagesController(NutriBestDbContext db, IPackageService packageService)
        {
            this.db = db;
            this.packageService = packageService;
        }

        [HttpGet]
        //[ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByHeader = "User-Agent")]
        public async Task<ActionResult<PackageServiceModel>> All()
        {
            try
            {
                var packages = await db.Packages
                    .Select(x => new PackageServiceModel
                    {
                        Grams = x.Grams
                    })
                    .ToListAsync();

                return Ok(packages);
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    Message = "Could not fetch packages!"
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult<int>> Create([FromForm] PackageServiceModel package)
        {
            try
            {
                if (package.Grams == 0)
                {
                    return BadRequest(new
                    {
                        Message = "Invalid Grams!"
                    });
                }

                var packageId = await packageService.Create(package.Grams);

                return Ok(packageId);
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new
                {
                    err.Message
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("{grams}")]
        public async Task<ActionResult<bool>> Remove([FromRoute] int grams)
        {
            try
            {
                var result = await packageService.Remove(grams);

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
