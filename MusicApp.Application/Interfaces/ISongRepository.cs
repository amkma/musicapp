using MusicApp.Domain.Entities;

namespace MusicApp.Application.Interfaces;

public interface ISongRepository
{
    Task<IReadOnlyList<Song>> GetPageAsync(
        string? singerName,
        int? categoryId,
        int pageSize,
        int pageIndex,
        int? lastId,
        string? seek,
        CancellationToken ct = default);

    Task<int> CountAsync(string? singerName, int? categoryId, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<Song> songs, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
