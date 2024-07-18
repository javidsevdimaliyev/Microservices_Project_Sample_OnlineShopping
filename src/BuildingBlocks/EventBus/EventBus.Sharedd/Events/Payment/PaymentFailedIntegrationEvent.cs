using EventBus.Base.Events;
using EventBus.Shared.Messages;

namespace EventBus.Shared.Events.Payment;

public class PaymentFailedIntegrationEvent : IntegrationEvent
{
    public int BuyerId { get; set; }
    public Guid OrderId { get; }

    public string ErrorMessage { get; }

    public List<OrderItemMessage> OrderItems { get; set; }

    public PaymentFailedIntegrationEvent(Guid orderId, string errorMessage)
    {
        OrderItems=new List<OrderItemMessage>();
        ErrorMessage = errorMessage ?? string.Empty;
        OrderId = orderId;
    }
}
