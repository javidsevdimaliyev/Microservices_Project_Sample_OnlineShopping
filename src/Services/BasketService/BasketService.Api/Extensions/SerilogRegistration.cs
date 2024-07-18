using Serilog;

namespace BasketService.Api.Extensions
{
    public static class SerilogRegistration
    {
        public static WebApplicationBuilder AddSerilogConfiguration(this WebApplicationBuilder builder)
        {

            var aspnetcorenv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var dotnetcorenv = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            var environment = aspnetcorenv ?? dotnetcorenv;
            builder.Configuration.AddCommandLine(Environment.GetCommandLineArgs())
                   .AddJsonFile("serilog.json", optional: false, reloadOnChange: true)
                   .AddJsonFile($"serilog.{environment}.json", optional: true)
                   .AddEnvironmentVariables();

            builder.Logging.ClearProviders();

            builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console());

            return builder;
        }      
    }
}
