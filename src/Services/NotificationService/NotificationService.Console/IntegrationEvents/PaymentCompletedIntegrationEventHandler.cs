using EventBus.Base.Abstraction;
using EventBus.Shared.Events.Payment;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace NotificationService.Console.IntegrationEvents;

public class PaymentCompletedIntegrationEventHandler : IIntegrationEventHandler<PaymentCompletedIntegrationEvent>, IConsumer<PaymentCompletedIntegrationEvent>
{
    private readonly ILogger<PaymentCompletedIntegrationEventHandler> _logger;

    public PaymentCompletedIntegrationEventHandler(ILogger<PaymentCompletedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentCompletedIntegrationEvent> context)
    {
        await Handle(context.Message);
    }

    public Task Handle(PaymentCompletedIntegrationEvent @event)
    {
        // Send success notification via Sms, Email or Push..

        _logger.LogInformation($"Order Payment successfullly completed with OrderId: {@event.OrderId}");

        return Task.CompletedTask;
    }
}
