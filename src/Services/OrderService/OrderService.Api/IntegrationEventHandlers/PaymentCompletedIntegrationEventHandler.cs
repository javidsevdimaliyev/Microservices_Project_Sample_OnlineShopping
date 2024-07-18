using EventBus.Base.Abstraction;
using EventBus.Shared.Events.Payment;
using MassTransit;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.OrderAggregate;
using System.Threading;

namespace OrderService.Api.IntegrationEventHandlers
{
    public class PaymentCompletedIntegrationEventHandler : IIntegrationEventHandler<PaymentCompletedIntegrationEvent>, IConsumer<PaymentCompletedIntegrationEvent>
    {
        private readonly IOrderRepository _orderRepository;

        public PaymentCompletedIntegrationEventHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedIntegrationEvent> context)
        {
            await Handle(context.Message);
        }

        public async Task Handle(PaymentCompletedIntegrationEvent @event)
        {
            var order = await _orderRepository.GetByIdAsync(@event.OrderId);
            if (order == null)
                throw new NullReferenceException();

            order.OrderStatus = OrderStatusEnum.Completed;
            await _orderRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
