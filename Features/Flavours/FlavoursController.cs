using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriBest.Server.Data;
using NutriBest.Server.Features.Flavours.Models;

namespace NutriBest.Server.Features.Flavours
{
    public class FlavoursController : ApiController
    {
        private readonly NutriBestDbContext db;
        private readonly IFlavourService flavourService;

        public FlavoursController(NutriBestDbContext db, IFlavourService flavourService)
        {
            this.db = db;
            this.flavourService = flavourService;
        }

        [HttpGet]
        //[ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByHeader = "User-Agent")]
        public async Task<ActionResult<FlavourServiceModel>> All()
        {
            try
            {
                var flavours = await db.Flavours
                    .Select(x => new FlavourServiceModel
                    {
                        Name = x.FlavourName
                    })
                    .OrderBy(x => x.Name)
                    .ToListAsync();

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
            catch (Exception err)
            {
                return BadRequest();
            }
        }
    }
}
