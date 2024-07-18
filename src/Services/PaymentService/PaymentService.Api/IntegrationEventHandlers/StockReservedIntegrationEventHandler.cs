using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using EventBus.Shared.Events.Payment;
using EventBus.Shared.Events.Stock;
using MassTransit;

namespace PaymentService.Api.IntegrationEventHandlers;

public class StockReservedIntegrationEventHandler : IIntegrationEventHandler<StockReservedIntegrationEvent>, IConsumer<StockReservedIntegrationEvent>
{
    private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;
    private readonly ILogger<StockReservedIntegrationEventHandler> _logger;

    public StockReservedIntegrationEventHandler(
        IConfiguration configuration,
        IEventBus eventBus,
        ILogger<StockReservedIntegrationEventHandler> logger)
    {
        _configuration = configuration;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<StockReservedIntegrationEvent> context)
    {
        await Handle(context.Message);
    }

    public async Task Handle(StockReservedIntegrationEvent @event)
    {
        // Fake payment process
        Random random = new Random();
        bool paymentResult = random.Next(2) == 0;
       
        //bool paymentSuccessFlag = _configuration.GetValue<bool>("PaymentSuccess");

        IntegrationEvent paymentEvent = paymentResult
            ? new PaymentCompletedIntegrationEvent(@event.OrderId)
            : new PaymentFailedIntegrationEvent(@event.OrderId, $"Order payment failed with OrderId:{@event.OrderId}");

        _logger.LogInformation($"OrderCreatedIntegrationEventHandler in PaymentService is fired with PaymentSuccess: {paymentResult}, orderId: {@event.OrderId}");

        await _eventBus.Publish(paymentEvent);
    }
}
