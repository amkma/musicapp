using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MusicApp.Application.Interfaces;
using MusicApp.Domain.Entities;
using MusicApp.Infrastructure.Persistence;

namespace MusicApp.Infrastructure.Persistence;

public class SongRepository(AppDbContext db) : ISongRepository
{
    private IQueryable<Song> BuildQuery(string? singerName, int? categoryId)
    {
        IQueryable<Song> q = db.Songs
            .Include(s => s.Singer)
            .Include(s => s.Category)
            .AsNoTracking();

        // ponytail: filters chained via LINQ method syntax, both apply together
        if (!string.IsNullOrWhiteSpace(singerName))
            q = q.Where(s => s.Singer.Name.Contains(singerName));

        if (categoryId is not null)
            q = q.Where(s => s.CategoryId == categoryId);

        return q;
    }

    public async Task<IReadOnlyList<Song>> GetPageAsync(
        string? singerName, int? categoryId, int pageSize,
        int pageIndex, int? lastId, string? seek, CancellationToken ct = default)
    {
        if (seek == "next" && lastId is not null)
        {
            var q = BuildQuery(singerName, categoryId).Where(s => s.Id > lastId.Value);
            var items = await q.OrderBy(s => s.Id).Take(pageSize).ToListAsync(ct);
            return items;
        }

        if (seek == "prev" && lastId is not null)
        {
            // ponytail: lastId for prev actually carries firstId (handler passes it as lastId too for prev direction)
            var q = BuildQuery(singerName, categoryId).Where(s => s.Id < lastId.Value).OrderByDescending(s => s.Id);
            var desc = await q.Take(pageSize).ToListAsync(ct);
            return desc.OrderBy(s => s.Id).ToList();
        }

        // OFFSET/FETCH w/ deterministic ORDER BY Id
        var page = BuildQuery(singerName, categoryId)
            .OrderBy(s => s.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);

        return await page.ToListAsync(ct);
    }

    public Task<int> CountAsync(string? singerName, int? categoryId, CancellationToken ct = default)
        => BuildQuery(singerName, categoryId).CountAsync(ct);

    public async Task AddRangeAsync(IEnumerable<Song> songs, CancellationToken ct = default)
    {
        // ponytail: disable change tracking for bulk insert, caller restores after final save
        db.ChangeTracker.AutoDetectChangesEnabled = false;
        try
        {
            await db.Songs.AddRangeAsync(songs, ct);
            await db.SaveChangesAsync(ct);
            db.ChangeTracker.Clear();
        }
        finally
        {
            db.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
