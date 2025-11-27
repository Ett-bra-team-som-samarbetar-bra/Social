namespace SocialBackend.Models;

public class PaginatedList<T>(List<T> items, int pageIndex, int totalPages)
{
    public List<T> Items { get; } = items;
    public int PageIndex { get; } = pageIndex;
    public int TotalPages { get; } = totalPages;
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
}

public static class PaginationExtension
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageIndex, int pageSize)
    {
        return query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);
    }
}