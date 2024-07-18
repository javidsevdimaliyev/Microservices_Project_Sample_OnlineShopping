using MediatR;
using OrderService.Domain.AggregateModels.BuyerAggregate;

namespace OrderService.Domain.Events;

public class BuyerAndPaymentMethodVerifiedDomainEvent : INotification
{
    public Buyer Buyer { get; private set; }

    public CardInfo Payment { get; private set; }

    public Guid OrderId { get; private set; }

    public BuyerAndPaymentMethodVerifiedDomainEvent(
        Buyer buyer, CardInfo payment, Guid orderId)
    {
        Buyer = buyer;
        Payment = payment;
        OrderId = orderId;
    }
}
