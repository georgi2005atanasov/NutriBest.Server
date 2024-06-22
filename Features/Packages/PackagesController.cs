namespace NutriBest.Server.Features.Packages
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.Packages.Models;

    public class PackagesController : ApiController
    {
        private readonly IPackageService packageService;

        public PackagesController(IPackageService packageService)
        {
            this.packageService = packageService;
        }

        [HttpGet]
        public async Task<ActionResult<PackageServiceModel>> All()
        {
            try
            {
                var packages = await packageService.All();

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

        [HttpGet]
        [Route("/Products/ByQuantityCount")]
        public async Task<ActionResult<PackageCountServiceModel>> GetProductsCountByQuantity()
        {
            try
            {
                var products = await packageService.GetProductsCountByQuantity();

                return Ok(products);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
