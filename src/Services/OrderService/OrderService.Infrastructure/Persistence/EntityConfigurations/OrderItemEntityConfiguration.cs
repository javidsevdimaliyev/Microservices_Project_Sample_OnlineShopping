using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Infrastructure.Persistence.Context;

namespace OrderService.Infrastructure.Persistence.EntityConfigurations;

public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("orderitems", OrderDbContext.DEFAULT_SCHEMA);

        builder.HasKey(oi => oi.Id);
        builder.Property(oi => oi.Id).ValueGeneratedOnAdd();
        builder.Property<int>("OrderId").IsRequired();

        builder.Ignore(oi => oi.DomainEvents);
    }
}
