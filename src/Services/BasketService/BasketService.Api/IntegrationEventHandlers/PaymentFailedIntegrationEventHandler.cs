using BasketService.Api.Core.Application.Repository;
using EventBus.Base.Abstraction;
using EventBus.Shared.Events.Payment;
using MassTransit;

namespace BasketService.Api.IntegrationEvents;

public class PaymentFailedIntegrationEventHandler : IIntegrationEventHandler<PaymentFailedIntegrationEvent>, IConsumer<PaymentFailedIntegrationEvent>
{
    private readonly IBasketRepository _basketRepository;
    private readonly ILogger<PaymentFailedIntegrationEventHandler> _logger;

    public PaymentFailedIntegrationEventHandler(IBasketRepository basketRepository, ILogger<PaymentFailedIntegrationEventHandler> logger)
    {
        _basketRepository = basketRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentFailedIntegrationEvent> context)
    {
        await Handle(context.Message);
    }


    public async Task Handle(PaymentFailedIntegrationEvent @event)
    {
        _logger.LogInformation($"--- Handling integration event: {@event.Id} at BasketService");

        await _basketRepository.DeleteBasketAsync(@event.BuyerId.ToString());
    }
}
