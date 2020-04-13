using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace Service.Infrastructure.Filters
{
    /// <summary>
    /// Header parameter filter for swagger
    /// </summary>
    public class HeaderParameterOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var filterDescriptors = context.ApiDescription.ActionDescriptor.FilterDescriptors;

            var allowAnonymous = filterDescriptors.Select(filterInfo => filterInfo.Filter).Any(filter => filter is IAllowAnonymousFilter);
            var validateBuUnit = filterDescriptors.Select(filterInfo => filterInfo.Filter).Any(filter => filter is ValidateBuUnitAttribute);

            // adding 401 responses for all [Authorize] attribute methods.
            if (context.MethodInfo.DeclaringType != null)
            {
                var authAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                    .Union(context.MethodInfo.GetCustomAttributes(true))
                    .OfType<AuthorizeAttribute>();

                if (authAttributes.Any() && !allowAnonymous)
                {
                    operation.Responses.Add("401", new OpenApiResponse {Description = "Unauthorized"});
                }
            }

            if (validateBuUnit && !allowAnonymous)
            {
                if (operation.Parameters == null)
                {
                    operation.Parameters = new List<OpenApiParameter>();
                }

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "BusinessUnit",
                    In = ParameterLocation.Header,
                    Description = "Business Unit/Division",
                    Required = true,
                    AllowEmptyValue = false,
                    Schema = new OpenApiSchema
                    {
                        Title = "Business Unit",
                        Type = "string",
                        Default = new OpenApiString(string.Empty)
                    }
                });
            }
        }
    }
}
