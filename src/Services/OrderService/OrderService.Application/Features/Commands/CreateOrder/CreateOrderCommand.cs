using MediatR;
using OrderService.Application.Dtos;

namespace OrderService.Application.Features.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<bool>
{
   
    public int BuyerId { get; private set; }

    // Address
    public string City { get; private set; }

    public string Street { get; private set; }

    public string State { get; private set; }

    public string Country { get; private set; }

    public string ZipCode { get; private set; }

    // Card info
    public string CardNumber { get; private set; }

    public string CardHolderName { get; private set; }

    public DateTime CardExpiration { get; private set; }

    public string CardSecurityNumber { get; private set; }

    public int CardTypeId { get; private set; }

    // Order info
    private readonly List<OrderItemDto> _orderItems;

    public IEnumerable<OrderItemDto> OrderItems => _orderItems;

    public CreateOrderCommand()
    {
        _orderItems = new List<OrderItemDto>();
    }

    public CreateOrderCommand(
        int buyerId, string city, string street, 
        string state, string country, string zipCode, 
        string cardNumber, string cardHolderName, 
        DateTime cardExpiration, string cardSecurityNumber, 
        int cardTypeId, List<OrderItemDto> basketItems) : this()
    {
        _orderItems = basketItems;
        BuyerId = buyerId;
        City = city;
        Street = street;
        State = state;
        Country = country;
        ZipCode = zipCode;
        CardNumber = cardNumber;
        CardHolderName = cardHolderName;
        CardExpiration = cardExpiration;
        CardSecurityNumber = cardSecurityNumber;
        CardTypeId = cardTypeId;
    }
}
