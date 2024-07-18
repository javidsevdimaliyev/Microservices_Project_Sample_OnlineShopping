using CatalogService.Domain.Entities;
using CatalogService.Infrastructure.Persistence.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Persistence.Context;

public class CatalogDbContext : DbContext
{
    public const string DEFAULT_SCHEMA = "catalog";

    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<CatalogBrandEntity> CatalogBrands { get; set; }
    public DbSet<CatalogItemEntity> CatalogItems { get; set; }
    public DbSet<CatalogTypeEntity> CatalogTypes { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Add your entity configurations here
        modelBuilder.ApplyConfiguration(new CatalogBrandEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogTypeEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogItemEntityTypeConfiguration());
    }
}