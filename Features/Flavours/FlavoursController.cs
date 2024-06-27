namespace NutriBest.Server.Features.Flavours
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using NutriBest.Server.Features.Flavours.Models;

    public class FlavoursController : ApiController
    {
        private readonly IFlavourService flavourService;

        public FlavoursController(IFlavourService flavourService)
        {
            this.flavourService = flavourService;
        }

        [HttpGet]
        //[ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByHeader = "User-Agent")]
        public async Task<ActionResult<List<FlavourServiceModel>>> All()
        {
            try
            {
                var flavours = await flavourService.All();

                return Ok(flavours);
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    Message = "Could not fetch flavours!"
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult<int>> Create([FromForm] FlavourServiceModel flavour)
        {
            try
            {
                var flavourId = await flavourService.Create(flavour.Name);

                return Ok(flavourId);
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
        [Route("{name}")]
        public async Task<ActionResult<bool>> Remove([FromRoute] string name)
        {
            try
            {
                var result = await flavourService.Remove(name);

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/Products/ByFlavourCount")]
        public async Task<ActionResult<List<FlavourCountServiceModel>>> GetProductsByFlavourCount()
        {
            try
            {
                var products = await flavourService.GetProductsByFlavourCount();

                return Ok(products);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
