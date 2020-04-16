using Mapdata.Api.Business;
using Mapdata.Api.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Infrastructure.Filters;
using Service.Infrastructure.Middlewares;
using Service.Infrastructure.Utils;

namespace Mapdata.Api
{

#pragma warning disable 1591
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => options.ReturnHttpNotAcceptable = true)
                .ConfigureApiBehaviorOptions(options =>
                    {
                        options.InvalidModelStateResponseFactory = ModelStateResponseFactory.InvalidModel;
                    })
                .AddNewtonsoftJson();

            services.AddMemoryCache();

            // Register db context
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = Configuration["DefaultConnection"] };
            var connectionString = connectionStringBuilder.ToString();

            var connection = new SqliteConnection(connectionString);

            services.AddDbContext<TnDistrictContext>(options =>
            {
                //options.UseSqlite("Filename=./Data/tamilnadu_data.sqlite");
                options.UseSqlite(Configuration["DefaultConnection"]);
            });

            // registers service related services
            // services.AddServiceServices(c =>
            // {
            //     Configuration.Bind(c);
            // });

            services.AddCors(s =>
            {
                s.AddDefaultPolicy(c =>
                {
                    c.AllowAnyOrigin();
                    c.AllowAnyHeader();
                    c.AllowAnyMethod();
                });
            });

            services.AddTransient<IEmailHelper>(provider =>
            {
                var emailHelper = new EmailHelper(Configuration["SmtpHost"]);

                return emailHelper;
            });

            // Register Business Class
            services.AddTransient<GeoJsonBusiness>();
            services.AddTransient<ChartDataBusiness>();
            services.AddTransient<GridDataBusiness>();
            services.AddTransient<CommentBusiness>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // app.UseServiceErrorLogging();
                app.UseDeveloperExceptionPage();
            }

            app.UseServiceRequestLogging();

            app.UseRouting();

            // global cors policy
            app.UseCors();

            app.UseHttpsRedirection();

            //app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapHealthChecks("/health");
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            // app.AddSwaggerUi();
        }
    }

#pragma warning restore 1591
}
