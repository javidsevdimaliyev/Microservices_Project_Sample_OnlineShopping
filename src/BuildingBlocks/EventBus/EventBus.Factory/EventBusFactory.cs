using EventBus.AzureServiceBus;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.MassTransit.RabbitMQ;
using EventBus.RabbitMQ;

namespace EventBus.Factory;

public static class EventBusFactory
{
    public static IEventBus Create(EventBusConfig eventBusConfig, IServiceProvider serviceProvider)
    {
        return eventBusConfig.EventBusType switch
        {
            EventBusType.AzureServiceBus => new EventBusServiceBus(serviceProvider, eventBusConfig),
            EventBusType.RabbitMQ => new EventBusRabbitMQ(serviceProvider, eventBusConfig),
            EventBusType.MassTransit_RabbitMQ => new EventBusMassTransitRabbitMQ(serviceProvider, eventBusConfig),
            _ => new EventBusRabbitMQ(serviceProvider, eventBusConfig)
        };
    }
}