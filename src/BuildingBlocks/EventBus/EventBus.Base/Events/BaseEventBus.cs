using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using EventBus.Base.Abstraction;
using EventBus.Base.SubManagers;

namespace EventBus.Base.Events;

public abstract class BaseEventBus : IEventBus
{
    public readonly IServiceProvider _serviceProvider;
    public readonly IEventBusSubscriptionManager SubscriptionManager;

    public EventBusConfig EventBusConfig { get; set; }

    protected BaseEventBus(IServiceProvider serviceProvider, EventBusConfig eventBusConfig)
    {
        EventBusConfig = eventBusConfig;
        _serviceProvider = serviceProvider;
        SubscriptionManager = new InMemoryEventBusSubscriptionManager(TrimEventName);
    }

    public abstract Task Publish(IntegrationEvent @event);
    public virtual async Task SendEndPoint(IntegrationEvent @event) { Publish(@event); }
    public abstract void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    public abstract void UnSubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;


    public virtual string TrimEventName(string eventName)
    {
        if (EventBusConfig.DeleteEventPrefix)
            eventName = eventName.TrimStart(EventBusConfig.EventNamePrefix.ToArray());

        if (EventBusConfig.DeleteEventSuffix)
            eventName = eventName.TrimEnd(EventBusConfig.EventNameSuffix.ToArray());

        return eventName;
    }

    public virtual string GetQueueName(string eventName)
    {
        return $"{EventBusConfig.SubscriberClientAppName}.{TrimEventName(eventName)}";
    }

    public virtual string GetRoutingKeyName(string eventName)
    {
        // ToDo: Refactoring about unique routing key with topic exchange
        return $".{TrimEventName(eventName)}";
    }


    public async Task<bool> HandleEvent(string eventName, string message)
    {
        eventName = TrimEventName(eventName);

        var processed = false;

        if (SubscriptionManager.HasSubscriptionsForEvent(eventName))
        {
            var subscriptions = SubscriptionManager.GetHandlersForEvent(eventName);

            using var scope = _serviceProvider.CreateScope();
            
            foreach (var subscription in subscriptions)
            {
                var handler = _serviceProvider.GetService(subscription.HandleType);
                if (handler == null) continue;

                var eventType = SubscriptionManager.GetEventTypeByName($"{EventBusConfig.EventNamePrefix}{eventName}{EventBusConfig.EventNameSuffix}");
                var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

                var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
            }

            processed = true;
        }

        return processed;
    }

    public virtual void Dispose()
    {
        EventBusConfig = null;
        SubscriptionManager.Clear();
    }


  
}
