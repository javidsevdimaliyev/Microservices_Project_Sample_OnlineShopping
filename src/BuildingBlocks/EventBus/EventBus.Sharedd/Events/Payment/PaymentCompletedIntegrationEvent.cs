using EventBus.Base.Events;

namespace EventBus.Shared.Events.Payment;

public class PaymentCompletedIntegrationEvent : IntegrationEvent
{
    public int BuyerId { get; set; }
    public Guid OrderId { get; }

    public PaymentCompletedIntegrationEvent(Guid orderId) => OrderId = orderId;
}
