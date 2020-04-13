using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Infrastructure.Models;
using System.Diagnostics;

namespace Service.Infrastructure.Filters
{
    public class ModelStateResponseFactory
    {
        /// <summary>
        /// Handles invalid model data exception
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IActionResult InvalidModel(ActionContext context)
        {
            var invalidModelError = new ValidationProblemDetails(context.ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
                Title = ErrorCodes.MODEL_VALIDATION,
                Status = StatusCodes.Status422UnprocessableEntity,
                Detail = "Model validation error",
                Instance = context.HttpContext.Request.Path
            };

            invalidModelError.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);
            invalidModelError.Extensions.Add("traceId", Activity.Current.RootId);

            return new UnprocessableEntityObjectResult(invalidModelError)
            {
                ContentTypes = {"application/problem+json"}
            };
        }
    }
}
