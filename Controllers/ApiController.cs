using Microsoft.AspNetCore.Mvc;

namespace NutriBest.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class ApiController : ControllerBase
    {
    }
}
