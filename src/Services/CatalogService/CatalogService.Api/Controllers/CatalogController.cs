using CatalogService.Api.ViewModels;
using CatalogService.Application.Dtos;
using CatalogService.Domain.Entities;
using CatalogService.Infrastructure.Persistence;
using CatalogService.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using Utilities;

namespace CatalogService.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CatalogController : ControllerBase
{
    private readonly CatalogDbContext _catalogContext;
    private readonly CatalogSettings _catalogSettings;

    public CatalogController(CatalogDbContext catalogContext, IOptionsSnapshot<CatalogSettings> catalogSettings)
    {
        _catalogContext = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
        _catalogSettings = catalogSettings.Value;

        catalogContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    
    /// <summary>
    ///     Retrieves paginated catalog items.
    /// </summary>
    /// <param name="pageIndex">Page index.</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>Paginated list of catalog items.</returns>
    /// <remarks>
    ///     Use this endpoint to retrieve a paginated list of catalog items.
    ///     Example request: GET /api/v1/catalog/items?pageIndex=1&amp;pageSize=10
    /// </remarks>
    [HttpGet("items")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItemDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IEnumerable<CatalogItemDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetCatalogItems([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, string ids = null)
    {
        if (!string.IsNullOrEmpty(ids))
        {
            var items = await GetItemsByIdAsync(ids);

            if (!items.Any())
            {
                return BadRequest("ids value is invalid. It must be comma-seperated list of numbers!");
            }

            return Ok(items);
        }

        var totalItems = await _catalogContext.CatalogItems
            .LongCountAsync();

        var itemsOnPage = await _catalogContext.CatalogItems
            .OrderBy(c => c.Name)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

      
        var itemDtosOnPage = ChangeUriPlaceHolder(itemsOnPage);

        var model = new PaginatedItemsViewModel<CatalogItemDto>(pageIndex, pageSize, totalItems, itemDtosOnPage);

        return Ok(model);
    }

    // GET -> api/v1/[controller]/items[?id=1]
    [HttpGet]
    [Route("items/{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(CatalogItemEntity), (int)HttpStatusCode.OK)]
    private async Task<ActionResult<CatalogItemEntity>> ItemByIdAsync(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid id");
        }

        var item = await _catalogContext.CatalogItems.SingleOrDefaultAsync(ci => ci.Id == id);

        var baseUri = _catalogSettings.PicBaseUrl;

        if (item != null)
        {
            item.PictureUri = baseUri + item.PictureFileName;

            return Ok(item);
        }

        return NotFound("No item with ID: " + id);
    }

    // GET -> api/v1/[controller]/items/withname/samplename[?pageIndex=5&pageSize=1]
    [HttpGet]
    [Route("items/withname/{name:minlength(1)}")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItemDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PaginatedItemsViewModel<CatalogItemDto>>> ItemsWithNameAsync(string name, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        var totalItems = await _catalogContext.CatalogItems
            .Where(c => c.Name.StartsWith(name))
            .LongCountAsync();

        var itemsOnPage = await _catalogContext.CatalogItems
            .OrderBy(c => c.Name.StartsWith(name))
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

      
        var itemDtosOnPage = ChangeUriPlaceHolder(itemsOnPage);

        var model = new PaginatedItemsViewModel<CatalogItemDto>(pageIndex, pageSize, totalItems, itemDtosOnPage);

        return Ok(model);
    }

    // GET -> api/v1/[controller]/items/type/1/brand/[?1&pageIndex=5&pageSize=1]
    [HttpGet]
    [Route("items/type/{catalogTypeId}/brand/{catalogBrandId:int?}")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItemDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PaginatedItemsViewModel<CatalogItemDto>>> ItemsByTypeIdAndBrandIdAsync(int catalogTypeId, int? catalogBrandId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        var root = (IQueryable<CatalogItemEntity>)_catalogContext.CatalogItems;

        root = root.Where(ci => ci.CatalogTypeId == catalogTypeId);

        if (catalogBrandId.HasValue)
        {
            root = root.Where(ci => ci.CatalogBrandId == catalogBrandId);
        }

        var totalItems = await root
            .LongCountAsync();

        var itemsOnPage = await root
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        var itemDtosOnPage = ChangeUriPlaceHolder(itemsOnPage);

        var model = new PaginatedItemsViewModel<CatalogItemDto>(pageIndex, pageSize, totalItems, itemDtosOnPage);

        return Ok(model);
    }

    // GET -> api/v1/[controller]/items/type/all/brand/[?1&pageIndex=5&pageSize=1]
    [HttpGet]
    [Route("items/type/all/brand/{catalogBrandId:int?}")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItemDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PaginatedItemsViewModel<CatalogItemDto>>> ItemsByBrandIdAsync(int? catalogBrandId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        var root = (IQueryable<CatalogItemEntity>)_catalogContext.CatalogItems;

        if (catalogBrandId.HasValue)
        {
            root = root.Where(ci => ci.CatalogBrandId == catalogBrandId);
        }

        var totalItems = await root
            .LongCountAsync();

        var itemsOnPage = await root
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        var itemDtosOnPage = ChangeUriPlaceHolder(itemsOnPage);

        var model = new PaginatedItemsViewModel<CatalogItemDto>(pageIndex, pageSize, totalItems, itemDtosOnPage);

        return Ok(model);
    }

    // GET -> api/v1/[controller]/catalogtypes
    [HttpGet]
    [Route("catalogtype")]
    [ProducesResponseType(typeof(List<CatalogTypeEntity>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<List<CatalogTypeEntity>>> CatalogTypesAsync()
    {
        return await _catalogContext.CatalogTypes.ToListAsync();
    }

    // GET -> api/v1/[controller]/catalogbrands
    [HttpGet]
    [Route("catalogbrands")]
    [ProducesResponseType(typeof(List<CatalogBrandEntity>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<List<CatalogBrandEntity>>> CatalogBrandsAsync()
    {
        return await _catalogContext.CatalogBrands.ToListAsync();
    }

    // PUT -> api/v1/[controller]/items
    [HttpPut]
    [Route("items")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> UpdateCatalogAsync([FromBody] CatalogItemEntity catalogToUpdate)
    {
        var catalogItem = await _catalogContext.CatalogItems.SingleOrDefaultAsync(ci => ci.Id == catalogToUpdate.Id);

        if (catalogItem == null) 
        {
            return NotFound(new { Message = $"Item with id {catalogToUpdate.Id} not found!" });
        }

        var oldPrice = catalogItem.Price;
        var raiseCatalogPriceChangedEvent = oldPrice != catalogToUpdate.Price;

        // Update current catalog
        catalogItem = catalogToUpdate;
        _catalogContext.CatalogItems.Update(catalogItem);

        await _catalogContext.SaveChangesAsync();

        return CreatedAtAction(nameof(ItemByIdAsync), new { id = catalogToUpdate.Id }, null);
    }

    // POST -> api/v1/[controller]/items
    [HttpPost]
    [Route("items")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateCatalogAsync([FromBody] CatalogItemEntity catalogToAdd)
    {
        var catalog = new CatalogItemEntity
        {
            CatalogBrandId = catalogToAdd.CatalogBrandId,
            CatalogTypeId = catalogToAdd.CatalogTypeId,
            Description = catalogToAdd.Description,
            Name = catalogToAdd.Name,
            PictureFileName = catalogToAdd.PictureFileName,
            Price = catalogToAdd.Price
        };

        await _catalogContext.CatalogItems.AddAsync(catalog);

        await _catalogContext.SaveChangesAsync();

        return CreatedAtAction(nameof(ItemByIdAsync), new { id = catalog.Id }, null);
    }

    // DELETE -> api/v1/[controller]/id
    [HttpDelete]
    [Route("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> DeleteCatalogAsync(int id)
    {
        var catalogToDelete = await _catalogContext.CatalogItems.SingleOrDefaultAsync(ci => ci.Id == id);

        if (catalogToDelete == null)
        {
            return NotFound($"Catalog with id:{id} could not found!");
        }

        _catalogContext.CatalogItems.Remove(catalogToDelete);

        await _catalogContext.SaveChangesAsync();

        return NoContent();
    }

    private async Task<List<CatalogItemDto>> GetItemsByIdAsync(string ids)
    {
        var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x));

        if (!numIds.All(nid => nid.Ok))
        {
            return new List<CatalogItemDto>();
        }

        var idsToSelect = numIds
            .Select(id => id.Value);

        var catalogs = await _catalogContext.CatalogItems.Where(ci => idsToSelect.Contains(ci.Id)).ToListAsync();

        var catalogvms = ChangeUriPlaceHolder(catalogs);

        return catalogvms;
    }

    private List<CatalogItemDto> ChangeUriPlaceHolder(List<CatalogItemEntity> itemsOnPage)
    {
        var baseUri = _catalogSettings.PicBaseUrl;

        foreach (var item in itemsOnPage)
        {
            if (item != null)
            {
                item.PictureUri = baseUri + item.PictureFileName;
            }
        }
  
        return itemsOnPage.Map<CatalogItemDto>().ToList(); ;
    }
}
