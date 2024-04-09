using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NutriBest.Server.Infrastructure.FIlters
{
    public class ModelOrNotFoundActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Result is ObjectResult result)
            {
                var model = result.Value;

                if (result == null)
                {
                    context.Result = new NotFoundResult();
                }
            }
        }
    }
}
