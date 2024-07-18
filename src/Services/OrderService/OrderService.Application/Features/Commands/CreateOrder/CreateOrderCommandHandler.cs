using EventBus.Base.Abstraction;
using EventBus.Shared.Events.Order;
using EventBus.Shared.Messages;
using MediatR;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Domain.ValueObjects;
using Utilities;

namespace OrderService.Application.Features.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEventBus _eventBus;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IEventBus eventBus)
    {
        _orderRepository = orderRepository;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var address = new Address(request.Street, request.City, request.State, request.Country, request.ZipCode);

        var dbOrder = new Order(request.BuyerId, address, request.CardTypeId,
            request.CardNumber, request.CardSecurityNumber, request.CardHolderName,
            request.CardExpiration);

        //foreach (var i in request.OrderItems)       
        //    dbOrder.AddOrderItem(i.ProductId, i.ProductName, i.UnitPrice, i.PictureUrl, i.Quantity);

        request.OrderItems.ToList()
            .ForEach(i => dbOrder.AddOrderItem(i.ProductId, i.ProductName, i.UnitPrice, i.PictureUrl, i.Quantity));
       
        await _orderRepository.AddAsync(dbOrder);
        await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        var orderCreatedIntegrationEvent = new OrderCreatedIntegrationEvent(
               request.BuyerId, dbOrder.Id, address.City,
               address.Street, address.State, address.Country,
               address.ZipCode, request.CardNumber, request.CardHolderName,
               request.CardSecurityNumber, request.CardTypeId, request.CardExpiration, request.OrderItems.Map<OrderItemMessage>().ToList());
       
        await _eventBus.Publish(orderCreatedIntegrationEvent);

        return true;
    }
}
