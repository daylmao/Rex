namespace Rex.Application.Pagination;

public class PagedResult<T>
{
    public PagedResult()
    {
        
    }
    public PagedResult(IEnumerable<T>? items, int totalItems, int actualPage, int pageSize)
    {
        Items = items;
        TotalItems = totalItems;
        ActualPage = actualPage;
        TotalPages = pageSize > 0
            ? (int)Math.Ceiling(totalItems / (double)pageSize)
            : 0;
    }
    
    public IEnumerable<T> Items { get; set; }
    public int TotalItems { get; set; }
    public int ActualPage { get; set; }
    public int TotalPages { get; set; }
    
}