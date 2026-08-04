using Microsoft.EntityFrameworkCore;
using MusicApp.Application.Interfaces;
using MusicApp.Domain.Entities;
using MusicApp.Infrastructure.Persistence;

namespace MusicApp.Infrastructure.Persistence;

public class SingerRepository(AppDbContext db) : ISingerRepository
{
    public async Task<IReadOnlyList<Singer>> GetPageAsync(int pageIndex, int pageSize, CancellationToken ct = default)
    {
        var items = await db.Singers
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return items;
    }

    public Task<Singer?> GetByIdAsync(int id, CancellationToken ct = default)
        => db.Singers.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<Singer?> GetByIdWithSongsAsync(int id, CancellationToken ct = default)
        => db.Singers
            .Include(s => s.Songs)
            .ThenInclude(s => s.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task AddAsync(Singer singer, CancellationToken ct = default)
    {
        db.Singers.Add(singer);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Singer singer, CancellationToken ct = default)
    {
        db.Update(singer);
        await db.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Singer singer, CancellationToken ct = default)
    {
        db.Singers.Remove(singer);
        await db.SaveChangesAsync(ct);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => db.Singers.AnyAsync(s => s.Id == id, ct);

    public Task<int> CountAsync(CancellationToken ct = default)
        => db.Singers.AsNoTracking().CountAsync(ct);
}
