using Microsoft.AspNetCore.Authorization;

namespace NutriBest.Server.Features.Admin
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : ApiController
    {

    }
}
