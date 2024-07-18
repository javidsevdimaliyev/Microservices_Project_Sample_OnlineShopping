using MediatR;
using OrderService.Domain.AggregateModels.OrderAggregate;

namespace OrderService.Domain.Events;

public class OrderStartedDomainEvent : INotification
{
    public int BuyerId { get; set; }

    public int CardTypeId { get; set; }

    public string CardNumber { get; set; }

    public string CardSecurityNumber { get; set; }

    public string CardHolderName { get; set; }

    public DateTime CardExpiration { get; set; }

    public Order Order { get; set; }

    public OrderStartedDomainEvent(
        int buyerId, int cardTypeId, string cardNumber, 
        string cardSecurityNumber, string cardHolderName, 
        DateTime cardExpiration, Order order)
    {
        BuyerId = buyerId;
        CardTypeId = cardTypeId;
        CardNumber = cardNumber;
        CardSecurityNumber = cardSecurityNumber;
        CardHolderName = cardHolderName;
        CardExpiration = cardExpiration;
        Order = order;
    }
}
