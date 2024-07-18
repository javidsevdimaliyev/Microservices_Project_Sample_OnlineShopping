using EventBus.Base.Abstraction;
using EventBus.Shared.Events.Payment;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace NotificationService.Console.IntegrationEvents;

public class PaymentFailedIntegrationEventHandler : IIntegrationEventHandler<PaymentFailedIntegrationEvent>, IConsumer<PaymentFailedIntegrationEvent>
{
    private readonly ILogger<PaymentFailedIntegrationEventHandler> _logger;

    public PaymentFailedIntegrationEventHandler(ILogger<PaymentFailedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<PaymentFailedIntegrationEvent> context)
    {
        await Handle(context.Message);
    }


    public Task Handle(PaymentFailedIntegrationEvent @event)
    {
        // Send failed notification via Sms, Email or Push..

        _logger.LogInformation($"Order Payment failed with OrderId: {@event.OrderId}, ErrorMessage: {@event.ErrorMessage}");

        return Task.CompletedTask;
    }
}
