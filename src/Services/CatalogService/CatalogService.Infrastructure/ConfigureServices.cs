using Microsoft.EntityFrameworkCore;
using CatalogService.Infrastructure.Persistence.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Console;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CatalogService.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MSSQL");

        services.AddEntityFrameworkSqlServer()
            .AddDbContext<CatalogDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
#if DEBUG
                    options.EnableDetailedErrors(); // To get field-level error details
                    options.EnableSensitiveDataLogging(); // To get parameter values - don't use this in production
                    options.ConfigureWarnings(warningAction =>
                    {
                        warningAction.Log(CoreEventId.FirstWithoutOrderByAndFilterWarning,
                            CoreEventId.RowLimitingOperationWithoutOrderByWarning);
                    });
#endif
                    sqlOptions.MigrationsAssembly(typeof(StartupBase).GetType().Assembly.GetName().Name);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });

                options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole())); // Add console logger
            });

        return services;
    }
}
