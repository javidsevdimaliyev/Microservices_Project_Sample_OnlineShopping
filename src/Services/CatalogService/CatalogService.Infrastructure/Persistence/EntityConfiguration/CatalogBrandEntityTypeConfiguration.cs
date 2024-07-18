using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CatalogService.Infrastructure.Persistence.Context;

namespace CatalogService.Infrastructure.Persistence.EntityConfiguration;

public class CatalogBrandEntityTypeConfiguration : IEntityTypeConfiguration<CatalogBrandEntity>
{
    public void Configure(EntityTypeBuilder<CatalogBrandEntity> builder)
    {
        builder.ToTable(nameof(CatalogBrandEntity), CatalogDbContext.DEFAULT_SCHEMA);

        builder.HasKey(cb => cb.Id);

        builder.Property(cb => cb.Id)
            .UseHiLo("catalog_brand_hilo")
            .IsRequired();

        builder.Property(cb => cb.Brand)
            .IsRequired()
            .HasMaxLength(100);
    }
}
