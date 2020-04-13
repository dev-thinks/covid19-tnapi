using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace Mapdata.Api
{

#pragma warning disable 1591

    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                var hostBuilder = new WebHostBuilder();
                var environment = hostBuilder.GetSetting("environment");

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environment}.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .WriteTo.Console()
                    .WriteTo.EventCollector(configuration["SplunkHost"],
                        configuration["SplunkToken"],
                        configuration["SplunkURI"],
                        configuration["SplunkSource"],
                        configuration["SplunkSourceType"],
                        renderTemplate: false)
                    .CreateLogger();
            }
            catch (Exception e)
            {
                // when logger is not initialized, exception will sent to console.
                Console.WriteLine(e);
            }

            try
            {
                Log.Information("[Mapdata.Api] Starting web host.");
                CreateHostBuilder(args).Build().Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[Mapdata.Api] Host terminated unexpectedly.");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(config => { config.ClearProviders(); })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseSerilog(dispose: true);
                });
    }

#pragma warning disable 1591

}
