using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedDll.Errors;

namespace WebApiCore10.RustApi.Presentation.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var apiError = new ErrorResponse
                {
                    StatusCode = 400,
                    StatusPhrase = "Bad Request",
                    Timestamp = DateTime.UtcNow,
                    Path = context.HttpContext.Request.Path,
                    Method = context.HttpContext.Request.Method,
                    Detail = "Validation failed.",
                    CorrelationId = context.HttpContext.TraceIdentifier
                };

                foreach (var error in context.ModelState.Values.SelectMany(v => v.Errors))
                {
                    apiError.Errors.Add(error.ErrorMessage);
                }

                context.Result = new BadRequestObjectResult(apiError);
            }
        }

    }
}
