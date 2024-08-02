using EventBus.Base.Abstraction;
using EventBus.UnitTest.Events.Events;
using MassTransit;

namespace EventBus.UnitTest.Events.EventHandlers;

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>, IConsumer<OrderCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        await Handle(context.Message);
    }

    public Task Handle(OrderCreatedIntegrationEvent @event)
    {
        Console.WriteLine($"Handle method triggered with id:{@event.Id}");

        return Task.CompletedTask;
    }
}
