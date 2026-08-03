using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MusicApp.Web.Helpers;

// ponytail: deterministic ORDER BY indexed Id BEFORE Skip/Take -> deterministic OFFSET/FETCH.
// Count cached via IMemoryCache w/ TTL to avoid full COUNT(*) on every page request.
// Sequential Next/Prev handled by caller using seek predicates (WHERE Id > lastId).
public class PaginatedList<T>(IReadOnlyList<T> items, int count, int pageIndex, int pageSize)
{
    public IReadOnlyList<T> Items { get; } = items;
    public int PageIndex { get; } = pageIndex;
    public int PageSize { get; } = pageSize;
    public int TotalCount { get; } = count;
    public int TotalPages { get; } = (int)Math.Ceiling(count / (double)pageSize);
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source,
        Expression<Func<T, int>> orderKey,
        int pageIndex,
        int pageSize,
        IMemoryCache? cache = null,
        string? cacheKey = null,
        TimeSpan cacheTtl = default)
    {
        int count = await GetTotalCountAsync(source, cache, cacheKey, cacheTtl);

        var items = await source
            .OrderBy(orderKey)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }

    private static async Task<int> GetTotalCountAsync(
        IQueryable<T> source,
        IMemoryCache? cache,
        string? cacheKey,
        TimeSpan cacheTtl)
    {
        if (cache is not null && cacheKey is not null && cacheTtl != default
            && cache.TryGetValue(cacheKey, out int cached) && cached > 0)
            return cached;

        int count = await source.CountAsync();

        if (cache is not null && cacheKey is not null && cacheTtl != default)
            cache.Set(cacheKey, count, cacheTtl);

        return count;
    }
}
