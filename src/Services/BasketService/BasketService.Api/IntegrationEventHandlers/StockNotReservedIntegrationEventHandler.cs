using BasketService.Api.Core.Application.Repository;
using EventBus.Base.Abstraction;
using EventBus.Shared.Events.Stock;
using MassTransit;

namespace BasketService.Api.IntegrationEventHandlers
{
    public class StockNotReservedIntegrationEventHandler : IIntegrationEventHandler<StockNotReservedIntegrationEvent>, IConsumer<StockNotReservedIntegrationEvent>
    {
    
        private readonly IBasketRepository _basketRepository;
        private readonly ILogger<StockNotReservedIntegrationEventHandler> _logger;

        public StockNotReservedIntegrationEventHandler(IBasketRepository basketRepository, ILogger<StockNotReservedIntegrationEventHandler> logger)
        {
            _basketRepository = basketRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StockNotReservedIntegrationEvent> context)
        {
            await Handle(context.Message);
        }


        public async Task Handle(StockNotReservedIntegrationEvent @event)
        {
            _logger.LogInformation($"--- Handling integration event: {@event.Id} at BasketService");

            await _basketRepository.DeleteBasketAsync(@event.BuyerId.ToString());
        }
    }
}
