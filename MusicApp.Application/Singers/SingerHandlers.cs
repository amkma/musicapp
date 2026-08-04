using MusicApp.Domain.Entities;
using MusicApp.Application.Interfaces;

namespace MusicApp.Application.Singers;

public record SingerPage(IReadOnlyList<Singer> Items, int TotalCount, int PageIndex, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
}

public class SingerHandlers(ISingerRepository repo)
{
    public async Task<SingerPage> ListAsync(int page, int pageSize = 100, CancellationToken ct = default)
    {
        int pageIndex = page < 1 ? 1 : page;
        var items = await repo.GetPageAsync(pageIndex, pageSize, ct);
        int total = await repo.CountAsync(ct);
        return new SingerPage(items, total, pageIndex, pageSize);
    }

    public Task<Singer?> DetailsAsync(int id, CancellationToken ct = default)
        => repo.GetByIdWithSongsAsync(id, ct);

    public Task CreateAsync(Singer singer, CancellationToken ct = default)
        => repo.AddAsync(singer, ct);

    public async Task<bool> EditAsync(Singer singer, CancellationToken ct = default)
    {
        var existing = await repo.GetByIdAsync(singer.Id, ct);
        if (existing is null) return false;
        await repo.UpdateAsync(singer, ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var singer = await repo.GetByIdAsync(id, ct);
        if (singer is null) return false;
        await repo.RemoveAsync(singer, ct);
        return true;
    }
}
