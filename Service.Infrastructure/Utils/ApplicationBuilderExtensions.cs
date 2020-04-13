using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Service.Infrastructure.Utils
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder AddSwaggerUi(this IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Service Microservice API V1";
                c.DocExpansion(DocExpansion.List);
                c.DefaultModelExpandDepth(2);
                c.DefaultModelRendering(ModelRendering.Example);
                c.EnableDeepLinking();
                c.DisplayOperationId();
                c.EnableFilter();
                c.DisplayRequestDuration();

                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Service Microservice API V1");
            });

            return app;
        }
    }
}
