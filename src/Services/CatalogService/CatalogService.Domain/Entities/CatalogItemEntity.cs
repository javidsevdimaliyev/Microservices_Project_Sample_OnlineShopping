namespace CatalogService.Domain.Entities;

public class CatalogItemEntity
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public string PictureFileName { get; set; }

    public string PictureUri { get; set; }

    public int AvailableStock { get; set; } = 0;

    public bool OnReorder { get; set; } = false;

    public int CatalogTypeId { get; set; }

    public CatalogTypeEntity CatalogTypeEntity { get; set; }

    public int CatalogBrandId { get; set; }

    public CatalogBrandEntity CatalogBrand { get; set; }
}