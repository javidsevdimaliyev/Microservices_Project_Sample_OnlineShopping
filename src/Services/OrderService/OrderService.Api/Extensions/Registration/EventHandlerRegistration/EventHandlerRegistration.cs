using OrderService.Api.IntegrationEventHandlers;

namespace OrderService.Api.Extensions.Registration.EventHandlerRegistration;

public static class EventHandlerRegistration
{
    public static IServiceCollection ConfigureEventHandlers(this IServiceCollection services)
    {
        services.AddTransient<OrderPreparedIntegrationEventHandler>();

        return services;
    }
}
