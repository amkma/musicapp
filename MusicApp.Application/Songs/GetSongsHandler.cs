using Microsoft.Extensions.Caching.Memory;
using MusicApp.Application.Interfaces;

namespace MusicApp.Application.Songs;

public class GetSongsHandler(ISongRepository repo, IMemoryCache cache)
{
    private const int PageSize = 100;
    private static readonly TimeSpan CountCacheTtl = TimeSpan.FromMinutes(5);

    public async Task<SongsPage> HandleAsync(GetSongsQuery q, CancellationToken ct = default)
    {
        string cacheKey = $"songs_count:{q.SingerName ?? ""}|{q.CategoryId ?? 0}";
        int pageIndex = q.Page is null or < 1 ? 1 : q.Page.Value;

        // ponytail: count cached per filter key; first hit pays COUNT(*), rest from cache
        int count = await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            int c = await repo.CountAsync(q.SingerName, q.CategoryId, ct);
            entry.AbsoluteExpirationRelativeToNow = CountCacheTtl;
            return c;
        });

        var items = await repo.GetPageAsync(
            q.SingerName, q.CategoryId, PageSize, pageIndex, q.LastId, q.Seek, ct);

        int firstId = items.Count > 0 ? items[0].Id : 0;
        int lastId = items.Count > 0 ? items[^1].Id : 0;

        // ponytail: seek path increments page index by exactly 1 per click
        if (q.Seek == "next" && q.LastId is not null)
            pageIndex = Math.Min((q.Page ?? 1) + 1, (int)Math.Ceiling(count / (double)PageSize));
        else if (q.Seek == "prev" && q.FirstId is not null)
            pageIndex = Math.Max(1, (q.Page ?? 2) - 1);

        return new SongsPage(items, count, pageIndex, PageSize, firstId, lastId);
    }
}
