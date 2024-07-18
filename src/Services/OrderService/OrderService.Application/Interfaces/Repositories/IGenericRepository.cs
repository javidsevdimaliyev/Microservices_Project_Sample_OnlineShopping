using OrderService.Domain.SeedWork;
using System.Linq.Expressions;

namespace OrderService.Application.Interfaces.Repositories;

public interface IGenericRepository<T,TKey> : IRepository<T, TKey> where T : BaseEntity<TKey>
{
    Task<List<T>> GetAllAsync();

    Task<List<T>> GetAsync(
        Expression<Func<T, bool>> filter = null, 
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
        params Expression<Func<T, object>>[] includes);

    Task<List<T>> GetAsync(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes);

    Task<T> GetByIdAsync(Guid id);

    Task<T> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);

    Task<T> GetSingleAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);

    Task<T> AddAsync(T entity);

    T Update(T entity);
}
