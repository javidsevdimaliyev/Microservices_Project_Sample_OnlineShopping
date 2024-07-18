using MediatR;

namespace OrderService.Domain.SeedWork;

public abstract class BaseEntity<T> : IBaseEntity
{
    public T Id { get; protected set; }
    public DateTime CreatedDate { get; set; }

    int? _requestedHashCode;

    private List<INotification> domainEvents;

    public IReadOnlyCollection<INotification> DomainEvents => domainEvents?.AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
        domainEvents = domainEvents ?? new List<INotification>();
        domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
        domainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        domainEvents?.Clear();
    }

    public bool IsTransient()
    {
        return Id.Equals(default(T));
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || !(obj is BaseEntity<T>))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (GetType() != obj.GetType())
            return false;

        BaseEntity<T> item = (BaseEntity<T>)obj;

        if (item.IsTransient() || IsTransient())
            return false;
        else
            return item.Id.Equals(Id);
    }

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
                _requestedHashCode = Id.GetHashCode() ^ 31; // XOR for random distribution

            return _requestedHashCode.Value;
        }
        else
            return base.GetHashCode();
    }

    public static bool operator ==(BaseEntity<T> left, BaseEntity<T> right)
    {
        if (Equals(left, null))
            return Equals(right, null) ? true : false;
        else
            return left.Equals(right);
    }

    public static bool operator !=(BaseEntity<T> left, BaseEntity<T> right)
    {
        return !(left == right);
    }
}
