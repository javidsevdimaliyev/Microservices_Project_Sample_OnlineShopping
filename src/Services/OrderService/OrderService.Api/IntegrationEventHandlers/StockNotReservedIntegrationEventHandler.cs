using Consul;
using EventBus.Base.Abstraction;
using EventBus.Shared.Events.Payment;
using EventBus.Shared.Events.Stock;
using MassTransit;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.OrderAggregate;

namespace OrderService.Api.IntegrationEventHandlers
{
    public class StockNotReservedIntegrationEventHandler : IIntegrationEventHandler<StockNotReservedIntegrationEvent>, IConsumer<StockNotReservedIntegrationEvent>
    {
        private readonly IOrderRepository _orderRepository;

        public StockNotReservedIntegrationEventHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task Consume(ConsumeContext<StockNotReservedIntegrationEvent> context)
        {
            await Handle(context.Message);
        }

        public async Task Handle(StockNotReservedIntegrationEvent @event)
        {
            var order = await _orderRepository.GetByIdAsync(@event.OrderId);
            if (order == null)
                throw new NullReferenceException();

            order.OrderStatus = OrderStatusEnum.Fail;
            await _orderRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
