using MusicApp.Domain.Entities;
using MusicApp.Application.Interfaces;

namespace MusicApp.Application.Categories;

public class CategoryHandlers(ICategoryRepository repo)
{
    public Task<IReadOnlyList<Category>> ListAsync(CancellationToken ct = default)
        => repo.GetAllAsync(ct);
}
