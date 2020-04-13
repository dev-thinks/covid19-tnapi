using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Service.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Service.Infrastructure.Filters
{
    /// <summary>
    /// Validates business unit header
    /// </summary>
    public class ValidateBuUnitAttribute : Attribute, IResourceFilter
    {
        private readonly string[] _headers;

        /// <summary>
        /// ctor
        /// </summary>
        public ValidateBuUnitAttribute()
        {
            _headers = new[] { "businessunit" };
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="headers"></param>
        public ValidateBuUnitAttribute(params string[] headers)
        {
            _headers = headers;
        }

        /// <summary>
        /// when resource executing
        /// </summary>
        /// <param name="context"></param>
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (!_headers.All(h => context.HttpContext.Request.Headers.ContainsKey(h)))
            {
                context.Result = FormErrorResult(context, ErrorCodes.CODE_502);
            }
            else if (_headers.All(h => context.HttpContext.Request.Headers.ContainsKey(h)))
            {
                context.HttpContext.Request.GetTypedHeaders().Headers.TryGetValue(_headers?.FirstOrDefault() ?? string.Empty, out var headerValue);

                // TODO:KK - validate against Division entity
                if (string.IsNullOrEmpty(headerValue.ToString()))
                {
                    context.Result = FormErrorResult(context, ErrorCodes.CODE_502);
                }
            }
        }

        /// <summary>
        /// when execution completes
        /// </summary>
        /// <param name="context"></param>
        public void OnResourceExecuted(ResourceExecutedContext context)
        {

        }

        private IActionResult FormErrorResult(ActionContext context, string code)
        {
            var invalidRequestModel = new ValidationProblemDetails(context.ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = code,
                Status = StatusCodes.Status400BadRequest,
                Instance = context.HttpContext.Request.Path,
                Errors = { new KeyValuePair<string, string[]>("Error Message", new[] { code }) }
            };

            invalidRequestModel.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);
            invalidRequestModel.Extensions.Add("traceId", Activity.Current.RootId);

            return new BadRequestObjectResult(invalidRequestModel)
            {
                ContentTypes = { "application/problem+json" }
            };
        }
    }
}