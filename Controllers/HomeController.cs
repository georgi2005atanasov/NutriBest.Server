using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NutriBest.Server.Controllers
{
    public class HomeController : ApiController
    {
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult Get()
        {
            return Ok("works!");
        }
    }
}