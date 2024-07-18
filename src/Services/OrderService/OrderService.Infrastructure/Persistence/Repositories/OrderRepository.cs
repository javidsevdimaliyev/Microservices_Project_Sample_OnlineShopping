using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Infrastructure.Persistence.Context;
using OrderService.Infrastructure.Persistence.Repositories.Common;
using System.Linq.Expressions;

namespace OrderService.Infrastructure.Persistence.Repositories;

public class OrderRepository : GenericRepository<Order, Guid>, IOrderRepository
{
    private readonly OrderDbContext _dbContext;

    public OrderRepository(OrderDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<Order> GetByIdAsync(Guid id, params Expression<Func<Order, object>>[] includes)
    {
        var entity = await base.GetByIdAsync(id, includes);

        if (entity == null)
        {
            entity = _dbContext.Orders.Local.FirstOrDefault(o => o.Id == id);
        }

        return entity;
    }
}
