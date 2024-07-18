namespace OrderService.Domain.SeedWork;

public interface IRepository<T, TKey>
{
    IUnitOfWork UnitOfWork { get; }
}
