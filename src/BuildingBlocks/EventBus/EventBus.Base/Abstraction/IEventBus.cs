using EventBus.Base.Events;

namespace EventBus.Base.Abstraction;

public interface IEventBus : IDisposable
{
    Task Publish(IntegrationEvent @event);

    Task SendEndPoint(IntegrationEvent @event);

    void Subscribe<T, TH>() 
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    void UnSubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;
}
