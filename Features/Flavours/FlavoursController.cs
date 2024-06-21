using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriBest.Server.Data;
using NutriBest.Server.Features.Flavours.Models;
using NutriBest.Server.Utilities;
using System.Text;

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


        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("CSV")]
        public async Task<FileContentResult?> GetCsv()
        {
            try
            {
                var flavours = await flavourService.All();
                var csv = ConvertToCsv(flavours);
                var bytes = Encoding.UTF8.GetBytes(csv);
                var result = new FileContentResult(bytes, "text/csv")
                {
                    FileDownloadName = "flavours.csv"
                };

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string ConvertToCsv(IEnumerable<FlavourServiceModel> flavours)
        {
            var csv = new StringBuilder();
            csv.AppendLine("FlavourName");

            foreach (var flavour in flavours)
            {
                csv.AppendLine(CsvHelper.EscapeCsvValue(flavour.Name));
            }

            return csv.ToString();
        }
    }
}
