using MusicApp.Domain.Entities;

namespace MusicApp.Application.Interfaces;

public interface ISingerRepository
{
    Task<IReadOnlyList<Singer>> GetPageAsync(int pageIndex, int pageSize, CancellationToken ct = default);
    Task<Singer?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Singer?> GetByIdWithSongsAsync(int id, CancellationToken ct = default);
    Task AddAsync(Singer singer, CancellationToken ct = default);
    Task UpdateAsync(Singer singer, CancellationToken ct = default);
    Task RemoveAsync(Singer singer, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);
}
