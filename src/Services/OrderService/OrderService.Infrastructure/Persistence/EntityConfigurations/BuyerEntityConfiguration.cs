using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Infrastructure.Persistence.Context;

namespace OrderService.Infrastructure.Persistence.EntityConfigurations;

public class BuyerEntityConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        builder.ToTable("buyers", OrderDbContext.DEFAULT_SCHEMA);

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedOnAdd();

        builder.Ignore(b => b.DomainEvents);

        builder.Property(b => b.Name)
               .HasColumnName("Name")
               .HasColumnType("varchar")
               .HasMaxLength(100);

        builder.HasMany(b => b.PaymentMethods)
               .WithOne()
               .HasForeignKey(b => b.Id)
               .OnDelete(DeleteBehavior.Cascade);

        var navigation = builder.Metadata.FindNavigation(nameof(Buyer.PaymentMethods));
        navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
