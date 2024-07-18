using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CatalogService.Infrastructure.Persistence.Context;

namespace CatalogService.Infrastructure.Persistence.EntityConfiguration;

public class CatalogTypeEntityTypeConfiguration : IEntityTypeConfiguration<CatalogTypeEntity>
{
    public void Configure(EntityTypeBuilder<CatalogTypeEntity> builder)
    {
        builder.ToTable(nameof(CatalogTypeEntity), CatalogDbContext.DEFAULT_SCHEMA);

        builder.HasKey(ct => ct.Id);

        builder.Property(ct => ct.Id)
            .UseHiLo("catalog_type_hilo")
            .IsRequired();

        builder.Property(ct => ct.Type)
            .IsRequired()
            .HasMaxLength(100);
    }
}
