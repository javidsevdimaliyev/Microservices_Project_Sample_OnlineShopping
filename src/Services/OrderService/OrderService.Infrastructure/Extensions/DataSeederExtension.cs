using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace OrderService.Infrastructure.Extensions;

public static class DataSeederExtension
{
    public static IApplicationBuilder MigrateDbContext<TContext>(
        this IApplicationBuilder app,
        Action<TContext, IServiceProvider> seeder)
        where TContext : DbContext
    {
        using var scope = app.ApplicationServices.CreateScope();

        var services = scope.ServiceProvider;

        var logger = services.GetService<ILogger<TContext>>();

        var context = services.GetRequiredService<TContext>();

        try
        {
            logger!.LogInformation("Migrating database associated with context {DbContextName}",
               typeof(TContext).Name);

            var retryIntervals = new[]
            {
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(6)
            };

            var retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetry(retryIntervals);

            retryPolicy.Execute(() => InvokeSeeder(seeder, context, services));

            logger!.LogInformation("Migrating database associated with context {DbContextName}",
                typeof(TContext).Name);
        }
        catch (Exception ex)
        {
            logger!.LogError(ex, $"An error occurred while migrating or seeding the {typeof(TContext).Name} database.");
        }

        return app;
    }

    private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context,
        IServiceProvider services) where TContext : DbContext
    {
        context.Database.EnsureCreated();
        context.Database.Migrate(); // Apply migrations

        seeder(context, services); // Seed data
    }
}