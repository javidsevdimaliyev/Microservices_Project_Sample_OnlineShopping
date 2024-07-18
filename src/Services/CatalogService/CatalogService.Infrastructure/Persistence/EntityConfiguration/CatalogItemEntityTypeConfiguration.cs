using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CatalogService.Infrastructure.Persistence.Context;

namespace CatalogService.Infrastructure.Persistence.EntityConfiguration;

public class CatalogItemEntityTypeConfiguration : IEntityTypeConfiguration<CatalogItemEntity>
{
    public void Configure(EntityTypeBuilder<CatalogItemEntity> builder)
    {
        builder.ToTable(nameof(CatalogItemEntity), CatalogDbContext.DEFAULT_SCHEMA);

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Id)
            .UseHiLo("catalog_item_hilo")
            .IsRequired();

        builder.Property(ci => ci.Name)
           .IsRequired()
           .HasMaxLength(50);

        builder.Property(ci => ci.Price)
            .IsRequired();

        builder.Property(ci => ci.Description)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(ci => ci.PictureFileName)
            .IsRequired(false);

        builder.Property(ci => ci.AvailableStock)
           .IsRequired();

        builder.Property(ci => ci.OnReorder)
           .IsRequired();

        builder.Ignore(ci => ci.PictureUri);

        builder.HasOne(ci => ci.CatalogBrand)
            .WithMany()
            .HasForeignKey(ci => ci.CatalogBrandId);

        builder.HasOne(ci => ci.CatalogTypeEntity)
            .WithMany()
            .HasForeignKey(ci => ci.CatalogTypeId);
    }
}
