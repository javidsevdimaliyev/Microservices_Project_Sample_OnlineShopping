using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Infrastructure.Persistence.Context;
using OrderService.Infrastructure.Persistence.Repositories.Common;

namespace OrderService.Infrastructure.Persistence.Repositories;

public class BuyerRepository : GenericRepository<Buyer, int>, IBuyerRepository
{
    public BuyerRepository(OrderDbContext dbContext) : base(dbContext)
    {

    }
}
