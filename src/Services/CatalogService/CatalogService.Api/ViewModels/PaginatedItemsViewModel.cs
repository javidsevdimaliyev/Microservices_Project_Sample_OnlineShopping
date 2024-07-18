namespace CatalogService.Api.ViewModels;

public class PaginatedItemsViewModel<TDto>
    where TDto : class
{
    public int PageIndex { get; private set; }

    public int PageSize { get; private set; }

    public long Count { get; private set; }

    public IEnumerable<TDto> Data { get; private set; }

    public PaginatedItemsViewModel(
        int pageIndex,
        int pageSize,
        long count,
        IEnumerable<TDto> data)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Count = count;
        Data = data;
    }
}
