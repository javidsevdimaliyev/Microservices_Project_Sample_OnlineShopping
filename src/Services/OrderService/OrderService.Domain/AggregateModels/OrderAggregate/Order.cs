using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.Events;
using OrderService.Domain.SeedWork;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.AggregateModels.OrderAggregate;

public class Order : BaseEntity<Guid>, IAggregateRoot
{  
    public DateTime OrderDate { get; private set; }

    public string Description { get; private set; }

    public int BuyerId { get; private set; }

    public Buyer Buyer { get; private set; }

    public Address Address { get; private set; }

    //private int orderStatusId;

    //public OrderStatus OrderStatus { get; private set; }
    public OrderStatusEnum OrderStatus { get; set; }

    private readonly List<OrderItem> _orderItems;

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

    public Guid? PaymentOperationId { get; set; }

    protected Order()
    {
        _orderItems = new List<OrderItem>();
    }

    public Order(int buyerId,
        Address address, int cartTypeId,
        string cardNumber, string CardSecurityNumber,
        string cardHolderName, DateTime cardExpiration) : this()
    {
        BuyerId = buyerId;
        //orderStatusId = OrderStatus.Submitted.Id;
        OrderStatus = OrderStatusEnum.Suspend;
        OrderDate = DateTime.UtcNow;
        Address = address;
        AddOrderStartedDomainEvent(buyerId,cartTypeId, cardNumber, CardSecurityNumber, cardHolderName, cardExpiration);
    }

    private void AddOrderStartedDomainEvent(int buyerId, int cartTypeId, 
                                            string cardNumber, string cardSecurityNumber, 
                                            string cardHolderName, DateTime cardExpiration)
    {
        var orderStartedDomainEvent = new OrderStartedDomainEvent(buyerId, cartTypeId,
                                                                  cardNumber, cardSecurityNumber,
                                                                  cardHolderName, cardExpiration, this);

        this.AddDomainEvent(orderStartedDomainEvent);
    }

    public void AddOrderItem(int productId, string productName, decimal unitPrice, string pictureUrl, int units = 1)
    {
        // ToDo: orderItem validations

        var orderItem = new OrderItem(productId, productName, pictureUrl, unitPrice, units);
        _orderItems.Add(orderItem);
    }

    public void SetBuyerId(int buyerId)
    {
        BuyerId = buyerId;
    }

    public void SetPaymentOperationId(Guid paymentOperationId)
    {
        PaymentOperationId = paymentOperationId;
    }
}

public enum OrderStatusEnum
{
    Suspend, Fail, Completed
}
