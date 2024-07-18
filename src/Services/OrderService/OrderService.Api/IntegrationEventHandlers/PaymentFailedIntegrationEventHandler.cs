using EventBus.Base.Abstraction;
using EventBus.Shared.Events.Payment;
using MassTransit;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.OrderAggregate;

namespace OrderService.Api.IntegrationEventHandlers
{
    public class PaymentFailedIntegrationEventHandler : IIntegrationEventHandler<PaymentFailedIntegrationEvent>, IConsumer<PaymentFailedIntegrationEvent>
    {
        private readonly IOrderRepository _orderRepository;

        public PaymentFailedIntegrationEventHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Consume(ConsumeContext<PaymentFailedIntegrationEvent> context)
        {
            await Handle(context.Message);
        }

        public async Task Handle(PaymentFailedIntegrationEvent @event)
        {
            var order = await _orderRepository.GetByIdAsync(@event.OrderId);
            if (order == null)
                throw new NullReferenceException();

            order.OrderStatus = OrderStatusEnum.Fail;
            await _orderRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
