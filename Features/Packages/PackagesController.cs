using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Packages
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using NutriBest.Server.Features.Packages.Models;
    using static ErrorMessages.PackagesController;
    using NutriBest.Server.Shared.Responses;

    public class PackagesController : ApiController
    {
        private readonly IPackageService packageService;

        public PackagesController(IPackageService packageService) 
            => this.packageService = packageService;

        [HttpGet]
        public async Task<ActionResult<List<PackageServiceModel>>> All()
        {
            try
            {
                var packages = await packageService.All();

                return Ok(packages);
            }
            catch (Exception)
            {
                return BadRequest(new FailResponse
                {
                    Message = CouldNotFetchPackages
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
                    return BadRequest(new FailResponse
                    {
                        Message = InvalidGrams
                    });
                }

                var packageId = await packageService.Create(package.Grams);

                return Ok(packageId);
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new FailResponse
                {
                    Message = err.Message
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
