using MediatR;
using OrderService.Domain.SeedWork;
using OrderService.Infrastructure.Persistence.Context;

namespace OrderService.Infrastructure.Extensions;

public static class MediatorExtension
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, OrderDbContext context)
    {
        var domainEntities = context.ChangeTracker
                                    .Entries<IBaseEntity>()
                                    .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

        var domainEvents = domainEntities.SelectMany(x => x.Entity.DomainEvents)
                                         .ToList();

        domainEntities.ToList()
                      .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach ( var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }
}
