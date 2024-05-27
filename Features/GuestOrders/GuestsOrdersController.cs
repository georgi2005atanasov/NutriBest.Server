namespace NutriBest.Server.Features.Orders
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class GuestsOrdersController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<int> Create()
        {
            return 1;
        } 
    }
}
