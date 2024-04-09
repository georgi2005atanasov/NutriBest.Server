using Microsoft.AspNetCore.Mvc;

namespace NutriBest.Server.Features
{
    [ApiController]
    [Route("[controller]")]
    public abstract class ApiController : ControllerBase
    {
    }
}
