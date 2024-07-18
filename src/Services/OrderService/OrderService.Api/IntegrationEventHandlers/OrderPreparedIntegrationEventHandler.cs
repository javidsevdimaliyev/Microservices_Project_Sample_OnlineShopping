using EventBus.Base.Abstraction;
using EventBus.Shared.Events.Order;
using MassTransit;
using MediatR;
using OrderService.Application.Dtos;
using OrderService.Application.Features.Commands.CreateOrder;
using System.Reflection;
using Utilities;

namespace OrderService.Api.IntegrationEventHandlers;

public class OrderPreparedIntegrationEventHandler : IIntegrationEventHandler<OrderPreparedIntegrationEvent>, IConsumer<OrderPreparedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderPreparedIntegrationEventHandler> _logger;

    public OrderPreparedIntegrationEventHandler(IMediator mediator, ILogger<OrderPreparedIntegrationEventHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<OrderPreparedIntegrationEvent> context)
    {
        await Handle(context.Message);
    }

    public async Task Handle(OrderPreparedIntegrationEvent @event)
    {
        _logger.LogInformation($"Processing integration event: {@event.Id} at {Assembly.GetExecutingAssembly().GetName().Name} - ({nameof(OrderCreatedIntegrationEvent)})");

        var createOrderCommand = new CreateOrderCommand(@event.BuyerId,
                @event.City, @event.Street,
                @event.State, @event.Country, @event.ZipCode,
                @event.CardNumber, @event.CardHolderName, @event.CardExpiration,
                @event.CardSecurityNumber, @event.CardTypeId, @event.OrderItems.Map<OrderItemDto>().ToList()
            );

        await _mediator.Send(createOrderCommand);
    }
}
