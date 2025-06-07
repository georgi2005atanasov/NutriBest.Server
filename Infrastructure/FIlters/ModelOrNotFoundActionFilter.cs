namespace NutriBest.Server.Infrastructure.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class ModelOrNotFoundActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Result is ObjectResult result)
            {
                var model = result.Value;

                if (result == null)
                    context.Result = new NotFoundResult();
            }
        }
    }
}
