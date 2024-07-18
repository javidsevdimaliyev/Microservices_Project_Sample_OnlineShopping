using System;

namespace BasketService.Api.Extensions
{
    public static class AppsettingsRegistration
    {
        public static WebApplicationBuilder SetEnvironment(this WebApplicationBuilder builder)
        {
           
            var aspnetcorenv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var dotnetcorenv = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            var environment = aspnetcorenv ?? dotnetcorenv;
            builder.Configuration.AddCommandLine(Environment.GetCommandLineArgs())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .AddJsonFile($"appsettings.{environment}.json", optional: true)
                   .AddEnvironmentVariables();

            return builder;
        }

      
    }
}
