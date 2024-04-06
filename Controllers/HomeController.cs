using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NutriBest.Server.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet]
        public ActionResult Get()
        {
            return Ok("works!");
        }
    }
}