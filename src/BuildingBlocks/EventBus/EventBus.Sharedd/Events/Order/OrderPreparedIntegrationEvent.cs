using EventBus.Base.Events;
using EventBus.Shared.Messages;

namespace EventBus.Shared.Events.Order;

public class OrderPreparedIntegrationEvent : IntegrationEvent
{
    // User info
    public int BuyerId { get; set; }


    // Address
    public string City { get; set; }

    public string Street { get; set; }

    public string State { get; set; }

    public string Country { get; set; }

    public string ZipCode { get; set; }

    // Card info
    public string CardNumber { get; set; }

    public string CardHolderName { get; set; }

    public string CardSecurityNumber { get; set; }

    public int CardTypeId { get; set; }

    public DateTime CardExpiration { get; set; }

    // Basket info
    // Order info
    private readonly List<OrderItemMessage> _orderItems;

    public IEnumerable<OrderItemMessage> OrderItems => _orderItems;

    public OrderPreparedIntegrationEvent()
    {
        _orderItems = new List<OrderItemMessage>();
    }

    //// Operational info
    //public Guid RequestId { get; set; }

    public OrderPreparedIntegrationEvent(int buyerId, string city, string street,
    string state, string country, string zipCode,
    string cardNumber, string cardHolderName, string cardSecurityNumber,
    int cardTypeId, DateTime cardExpiration,
    List<OrderItemMessage> basketItems)
    {
        BuyerId = buyerId;      
        City = city;
        Street = street;
        State = state;
        Country = country;
        ZipCode = zipCode;
        CardNumber = cardNumber;
        CardHolderName = cardHolderName;
        CardSecurityNumber = cardSecurityNumber;
        CardTypeId = cardTypeId;
        CardExpiration = cardExpiration;
        _orderItems = basketItems;
    }
}
