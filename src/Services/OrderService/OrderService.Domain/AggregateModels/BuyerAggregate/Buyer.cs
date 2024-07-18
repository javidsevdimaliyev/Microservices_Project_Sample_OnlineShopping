using OrderService.Domain.Events;
using OrderService.Domain.SeedWork;

namespace OrderService.Domain.AggregateModels.BuyerAggregate;

public class Buyer : BaseEntity<int>, IAggregateRoot
{
    public string Name { get; set; }

    private List<CardInfo> _paymentMethods;

    public IEnumerable<CardInfo> PaymentMethods => _paymentMethods.AsReadOnly();

    protected Buyer()
    {
        _paymentMethods = new List<CardInfo>();
    }

    public Buyer(string name) : this()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public CardInfo VerifyOrAddPaymentMethod(
        int cartTypeId, string alias, string cardNumber,
        string securityNumber, string cardHolderName,
        DateTime expiration, Guid orderId)
    {
        var existingPayment = _paymentMethods.SingleOrDefault(p => p.IsEqualTo(cartTypeId, cardNumber, expiration));

        if (existingPayment != null)
        {
            // raise event
            AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));

            return existingPayment;
        }

        var payment = new CardInfo(alias, cardNumber, securityNumber, cardHolderName, expiration, cartTypeId);

        _paymentMethods.Add(payment);

        // raise event
        AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, payment, orderId));

        return payment;
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj) || 
                (obj is Buyer buyer
                && Id.Equals(buyer.Id)
                && Name == buyer.Name);
    }
}
